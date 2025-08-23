using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CollectionApp.Application.Common;
using CollectionApp.Application.Interfaces;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CustomerDetailVM> CreateAsync(CustomerCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Check for duplicate national ID
            var existingCustomer = await _unitOfWork.Customers.GetByNationalIdAsync(model.NationalId);
            if (existingCustomer != null)
                throw new InvalidOperationException($"Customer with National ID {model.NationalId} already exists.");

            // Check for duplicate email if provided
            if (!string.IsNullOrEmpty(model.Email))
            {
                var existingEmailCustomer = await _unitOfWork.Customers.GetByEmailAsync(model.Email);
                if (existingEmailCustomer != null)
                    throw new InvalidOperationException($"Customer with email {model.Email} already exists.");
            }

            // Create address and phone value objects
            var address = new Address(model.Street, model.City, model.State, model.Country, model.ZipCode);
            var fullPhone = $"+{model.PhoneCountryCode}{model.PhoneAreaCode}{model.PhoneNumber}";
            var phone = new Phone(fullPhone, model.PhoneType);

            // Create customer entity
            var customer = new Customer(
                model.FirstName,
                model.LastName,
                model.NationalId,
                address,
                phone,
                model.Email,
                model.DateOfBirth,
                model.Gender,
                model.Occupation,
                model.EmployerName,
                model.MonthlyIncome,
                model.CreditScore,
                model.SourceOfFunds,
                model.PurposeOfLoan,
                model.Notes
            );

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CustomerDetailVM>(customer);
        }

        public async Task<CustomerDetailVM> UpdateAsync(CustomerUpdateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var customer = await _unitOfWork.Customers.GetByIdAsync(model.Id);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {model.Id} not found.");

            // Check for duplicate national ID if changed
            if (customer.NationalId != model.NationalId)
            {
                var existingCustomer = await _unitOfWork.Customers.GetByNationalIdAsync(model.NationalId);
                if (existingCustomer != null)
                    throw new InvalidOperationException($"Customer with National ID {model.NationalId} already exists.");
            }

            // Check for duplicate email if changed
            if (customer.Email != model.Email && !string.IsNullOrEmpty(model.Email))
            {
                var existingEmailCustomer = await _unitOfWork.Customers.GetByEmailAsync(model.Email);
                if (existingEmailCustomer != null)
                    throw new InvalidOperationException($"Customer with email {model.Email} already exists.");
            }

            // Update identity information
            customer.UpdateIdentity(model.FirstName, model.LastName, model.NationalId);

            // Update contact information
            var address = new Address(model.Street, model.City, model.State, model.Country, model.ZipCode);
            var fullPhone = $"+{model.PhoneCountryCode}{model.PhoneAreaCode}{model.PhoneNumber}";
            var phone = new Phone(fullPhone, model.PhoneType);
            customer.UpdateContact(address, phone, model.Email);

            // Update other properties
            customer.UpdatePersonalInfo(model.DateOfBirth, model.Gender, model.Occupation, model.EmployerName, model.MonthlyIncome, model.CreditScore, model.SourceOfFunds, model.PurposeOfLoan, model.Notes);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CustomerDetailVM>(customer);
        }

        public async Task<CustomerDetailVM> GetByIdAsync(Guid id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {id} not found.");

            return _mapper.Map<CustomerDetailVM>(customer);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {id} not found.");

            // Check if customer has active contracts
            var hasActiveContracts = await _unitOfWork.Contracts.HasActiveContractsAsync(id);
            if (hasActiveContracts)
                throw new InvalidOperationException("Cannot delete customer with active contracts.");

            await _unitOfWork.Customers.DeleteAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResult<CustomerListVM>> GetPagedAsync(string? searchTerm, int page, int pageSize, string? orderBy = null)
        {
            var filter = string.IsNullOrWhiteSpace(searchTerm) ? null : $"search={searchTerm}";
            var customers = await _unitOfWork.Customers.GetPagedAsync(filter, orderBy, page, pageSize);

            var customerListVMs = _mapper.Map<List<CustomerListVM>>(customers.Items);

            var items = customerListVMs != null ? customerListVMs.AsReadOnly() : new List<CustomerListVM>().AsReadOnly();
            return new PagedResult<CustomerListVM>(items, customers.TotalCount, customers.PageNumber, customers.PageSize);
        }

        public async Task<CustomerDetailVM> UpdateContactInfoAsync(Guid id, string street, string city, string state, string zipCode, string country, string phoneCountryCode, string phoneAreaCode, string phoneNumber, string phoneType, string email)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {id} not found.");

            var address = new Address(street, city, state, country, zipCode);
            var fullPhone = $"+{phoneCountryCode}{phoneAreaCode}{phoneNumber}";
            var phone = new Phone(fullPhone, phoneType);

            customer.UpdateContact(address, phone, email);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CustomerDetailVM>(customer);
        }

        public async Task<CustomerDetailVM> UpdateIdentityAsync(Guid id, string firstName, string lastName, string nationalId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {id} not found.");

            // Check for duplicate national ID if changed
            if (customer.NationalId != nationalId)
            {
                var existingCustomer = await _unitOfWork.Customers.GetByNationalIdAsync(nationalId);
                if (existingCustomer != null)
                    throw new InvalidOperationException($"Customer with National ID {nationalId} already exists.");
            }

            customer.UpdateIdentity(firstName, lastName, nationalId);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CustomerDetailVM>(customer);
        }

        public async Task<List<CustomerContractSummaryVM>> GetCustomerContractsAsync(Guid customerId)
        {
            var contracts = await _unitOfWork.Contracts.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<CustomerContractSummaryVM>>(contracts);
        }

        public async Task<PagedResult<CustomerListVM>> SearchCustomersAsync(string term, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return new PagedResult<CustomerListVM>(Array.Empty<CustomerListVM>(), 0, page, pageSize);
            }

            var filter = $"search={term}";
            var customers = await _unitOfWork.Customers.GetPagedAsync(filter, null, page, pageSize);
            var items = _mapper.Map<List<CustomerListVM>>(customers.Items) ?? new List<CustomerListVM>();
            return new PagedResult<CustomerListVM>(items.AsReadOnly(), customers.TotalCount, customers.PageNumber, customers.PageSize);
        }

        public async Task<PagedResult<CustomerListVM>> AdvancedSearchAsync(AdvancedCustomerSearchCriteriaVM criteria, int page, int pageSize)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            // Build encoded, culture-invariant filter string
            var parts = new List<string>();
            string Enc(string s) => Uri.EscapeDataString(s);
            void Add(string key, string? value)
            {
                if (!string.IsNullOrWhiteSpace(value)) parts.Add($"{key}={Enc(value)}");
            }
            void AddDec(string key, decimal? value)
            {
                if (value.HasValue) parts.Add($"{key}={value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }
            void AddInt(string key, int? value)
            {
                if (value.HasValue) parts.Add($"{key}={value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }
            void AddDate(string key, DateTime? value)
            {
                if (value.HasValue) parts.Add($"{key}={value.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}");
            }

            Add("firstName", criteria.FirstName);
            Add("lastName", criteria.LastName);
            Add("nationalId", criteria.NationalId);
            Add("email", criteria.Email);
            Add("gender", criteria.Gender);
            Add("occupation", criteria.Occupation);
            Add("city", criteria.City);
            Add("state", criteria.State);
            Add("country", criteria.Country);
            AddDate("dobFrom", criteria.DateOfBirthFrom);
            AddDate("dobTo", criteria.DateOfBirthTo);
            AddDec("incomeFrom", criteria.MonthlyIncomeFrom);
            AddDec("incomeTo", criteria.MonthlyIncomeTo);
            AddInt("creditFrom", criteria.CreditScoreFrom);
            AddInt("creditTo", criteria.CreditScoreTo);
            AddInt("activeContractsFrom", criteria.ActiveContractsCountFrom);
            AddInt("activeContractsTo", criteria.ActiveContractsCountTo);
            AddDec("totalContractValueFrom", criteria.TotalContractValueFrom);
            AddDec("totalContractValueTo", criteria.TotalContractValueTo);
            AddDate("createdFrom", criteria.CreatedAtFrom);
            AddDate("createdTo", criteria.CreatedAtTo);
            AddDate("updatedFrom", criteria.UpdatedAtFrom);
            AddDate("updatedTo", criteria.UpdatedAtTo);

            var filter = parts.Count == 0 ? null : string.Join(";", parts);
            var orderBy = string.IsNullOrWhiteSpace(criteria.OrderBy) ? null : criteria.OrderDescending ? $"{criteria.OrderBy} desc" : criteria.OrderBy;

            var customers = await _unitOfWork.Customers.GetPagedAsync(filter, orderBy, page, pageSize);
            var mapped = _mapper.Map<List<CustomerSearchResultVM>>(customers.Items) ?? new List<CustomerSearchResultVM>();
            return new PagedResult<CustomerListVM>(mapped.AsReadOnly(), customers.TotalCount, customers.PageNumber, customers.PageSize);
        }

        public async Task<CustomerAnalyticsVM> GetCustomerAnalyticsAsync(Guid? customerId = null)
        {
            // Basic analytics implementation
            var customersQuery = _unitOfWork.Customers.Query();
            if (customerId.HasValue)
            {
                customersQuery = customersQuery.Where(c => c.Id == customerId.Value);
            }

            var now = DateTime.UtcNow.Date;
            int Age(DateTime? dob)
            {
                if (!dob.HasValue) return -1;
                var a = now.Year - dob.Value.Year;
                if (dob.Value.Date > now.AddYears(-a)) a--;
                return a;
            }

            var list = customersQuery.ToList();

            var vm = new CustomerAnalyticsVM
            {
                TotalCustomers = list.Count,
                MaleCustomers = list.Count(c => string.Equals(c.Gender, "Male", StringComparison.OrdinalIgnoreCase)),
                FemaleCustomers = list.Count(c => string.Equals(c.Gender, "Female", StringComparison.OrdinalIgnoreCase)),
                OtherGenderCustomers = list.Count(c => !string.IsNullOrWhiteSpace(c.Gender) &&
                    !string.Equals(c.Gender, "Male", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(c.Gender, "Female", StringComparison.OrdinalIgnoreCase)),
                AverageMonthlyIncome = list.Any() ? list.Where(c => c.MonthlyIncome.HasValue).DefaultIfEmpty().Average(c => c?.MonthlyIncome ?? 0) : 0,
                CreditScoreLow = list.Count(c => c.CreditScore.HasValue && c.CreditScore.Value < 580),
                CreditScoreMedium = list.Count(c => c.CreditScore.HasValue && c.CreditScore.Value >= 580 && c.CreditScore.Value < 700),
                CreditScoreHigh = list.Count(c => c.CreditScore.HasValue && c.CreditScore.Value >= 700)
            };

            vm.AgeUnder25 = list.Count(c => { var a = Age(c.DateOfBirth); return a >= 0 && a < 25; });
            vm.Age25To34 = list.Count(c => { var a = Age(c.DateOfBirth); return a >= 25 && a <= 34; });
            vm.Age35To44 = list.Count(c => { var a = Age(c.DateOfBirth); return a >= 35 && a <= 44; });
            vm.Age45To54 = list.Count(c => { var a = Age(c.DateOfBirth); return a >= 45 && a <= 54; });
            vm.Age55Plus = list.Count(c => { var a = Age(c.DateOfBirth); return a >= 55; });

            // Groupings
            vm.CustomersByCountry = list
                .GroupBy(c => c.Address.Country ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.Count());
            vm.CustomersByState = list
                .GroupBy(c => c.Address.State ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.Count());
            vm.CustomersByCity = list
                .GroupBy(c => c.Address.City ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.Count());

            // Contracts-based analytics
            var contracts = _unitOfWork.Contracts.Query();
            if (customerId.HasValue)
            {
                contracts = contracts.Where(c => c.CustomerId == customerId.Value);
            }
            var contractsList = contracts.ToList();
            vm.TotalPortfolioValue = contractsList.Sum(c => c.TotalAmount.Amount);
            vm.ActiveContracts = contractsList.Count(c => c.Status == CollectionApp.Domain.Enums.ContractStatus.Active);
            vm.CompletedContracts = contractsList.Count(c => c.Status == CollectionApp.Domain.Enums.ContractStatus.Completed);
            vm.DefaultedContracts = contractsList.Count(c => c.Status == CollectionApp.Domain.Enums.ContractStatus.Defaulted);

            return vm;
        }

        public Task<CustomerAnalyticsVM> GetPortfolioAnalyticsAsync()
        {
            var vm = new CustomerAnalyticsVM();
            return Task.FromResult(vm);
        }

        public async Task<BulkUpdateResultVM> BulkUpdateCustomersAsync(BulkCustomerUpdateVM updateModel)
        {
            if (updateModel == null)
                throw new ArgumentNullException(nameof(updateModel));
            if (updateModel.CustomerIds == null || updateModel.CustomerIds.Count == 0)
                throw new InvalidOperationException("No customers selected for bulk update.");

            var result = new BulkUpdateResultVM { TotalRequested = updateModel.CustomerIds.Count };

            static string Pick(string? input, string existing, bool overwrite) =>
                string.IsNullOrWhiteSpace(input) && !overwrite ? existing : (input ?? existing);

            await _unitOfWork.ExecuteInTransactionAsync(async (CancellationToken ct) =>
            {
                foreach (var id in updateModel.CustomerIds)
                {
                    var validateOnly = updateModel.ValidateOnly;
                    var customer = validateOnly
                        ? await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.Id == id)
                        : await _unitOfWork.Customers.GetByIdAsync(id);
                    if (customer == null)
                    {
                        result.TotalSkipped++;
                        result.Messages.Add($"Customer {id} not found.");
                        continue;
                    }

                    // Contact with overwrite semantics
                    var street = Pick(updateModel.Street, customer.Address.Street, updateModel.OverwriteEmptyFields);
                    var city = Pick(updateModel.City, customer.Address.City, updateModel.OverwriteEmptyFields);
                    var state = Pick(updateModel.State, customer.Address.State, updateModel.OverwriteEmptyFields);
                    var country = Pick(updateModel.Country, customer.Address.Country, updateModel.OverwriteEmptyFields);
                    var zip = Pick(updateModel.ZipCode, customer.Address.PostalCode, updateModel.OverwriteEmptyFields);
                    var address = new Address(street, city, state, country, zip);

                    var phoneCc = Pick(updateModel.PhoneCountryCode, customer.Phone.CountryCode, updateModel.OverwriteEmptyFields);
                    var phoneAc = Pick(updateModel.PhoneAreaCode, customer.Phone.AreaCode, updateModel.OverwriteEmptyFields);
                    var phoneNum = Pick(updateModel.PhoneNumber, customer.Phone.Number, updateModel.OverwriteEmptyFields);
                    var phoneType = Pick(updateModel.PhoneType, customer.Phone.PhoneType, updateModel.OverwriteEmptyFields);
                    var phone = new Phone($"+{phoneCc}{phoneAc}{phoneNum}", phoneType);

                    var email = Pick(updateModel.Email, customer.Email, updateModel.OverwriteEmptyFields);

                    if (!validateOnly)
                    {
                        customer.UpdateContact(address, phone, email);
                        if (!string.IsNullOrWhiteSpace(updateModel.Notes))
                        {
                            customer.UpdatePersonalInfo(
                                customer.DateOfBirth,
                                customer.Gender,
                                customer.Occupation,
                                customer.EmployerName,
                                customer.MonthlyIncome,
                                customer.CreditScore,
                                customer.SourceOfFunds,
                                customer.PurposeOfLoan,
                                updateModel.Notes);
                        }
                    }

                    if (validateOnly)
                    {
                        // Count as updated if any field would change
                        var wouldChange = street != customer.Address.Street
                            || city != customer.Address.City
                            || state != customer.Address.State
                            || country != customer.Address.Country
                            || zip != customer.Address.PostalCode
                            || phoneCc != customer.Phone.CountryCode
                            || phoneAc != customer.Phone.AreaCode
                            || phoneNum != customer.Phone.Number
                            || phoneType != customer.Phone.PhoneType
                            || email != customer.Email
                            || (!string.IsNullOrWhiteSpace(updateModel.Notes)
                                && updateModel.Notes!.Trim() != (customer.Notes ?? string.Empty));

                        if (wouldChange)
                        {
                            result.TotalUpdated++;
                        }
                        else
                        {
                            result.TotalSkipped++;
                            result.Messages.Add($"Customer {id}: no changes.");
                        }
                        continue;
                    }
                    result.TotalUpdated++;
                }

                if (!updateModel.ValidateOnly)
                {
                    await _unitOfWork.SaveChangesAsync(ct);
                }

                return result;
            });

            return result;
        }

        public async Task<List<CustomerDetailVM>> BulkGetCustomersAsync(List<Guid> customerIds)
        {
            if (customerIds == null)
                throw new ArgumentNullException(nameof(customerIds));
            var entities = await _unitOfWork.Customers.GetByIdsAsync(customerIds);
            return _mapper.Map<List<CustomerDetailVM>>(entities) ?? new List<CustomerDetailVM>();
        }

        public async Task<CustomerDetailVM> LinkContractToCustomerAsync(Guid customerId, Guid contractId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async (CancellationToken ct) =>
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
                if (customer == null) throw new InvalidOperationException($"Customer with ID {customerId} not found.");
                var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
                if (contract == null) throw new InvalidOperationException($"Contract with ID {contractId} not found.");

                if (contract.CustomerId != Guid.Empty && contract.CustomerId == customerId)
                    throw new InvalidOperationException("Contract is already linked to this customer.");

                customer.AddContract(contract);
                await _unitOfWork.SaveChangesAsync(ct);
                return _mapper.Map<CustomerDetailVM>(customer);
            });
        }

        public async Task<CustomerDetailVM> TransferContractAsync(Guid contractId, Guid fromCustomerId, Guid toCustomerId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async (CancellationToken ct) =>
            {
                if (fromCustomerId == toCustomerId)
                    throw new InvalidOperationException("Source and target customers must be different.");

                var fromCustomer = await _unitOfWork.Customers.GetByIdAsync(fromCustomerId);
                if (fromCustomer == null)
                    throw new InvalidOperationException($"Customer with ID {fromCustomerId} not found.");

                var toCustomer = await _unitOfWork.Customers.GetByIdAsync(toCustomerId);
                if (toCustomer == null)
                    throw new InvalidOperationException($"Customer with ID {toCustomerId} not found.");

                var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
                if (contract == null)
                    throw new InvalidOperationException($"Contract with ID {contractId} not found.");

                if (contract.CustomerId != fromCustomerId)
                    throw new InvalidOperationException("The specified source customer does not own this contract.");

                toCustomer.AddContract(contract);
                await _unitOfWork.SaveChangesAsync(ct);
                return _mapper.Map<CustomerDetailVM>(toCustomer);
            });
        }

        public async Task<List<CustomerContractSummaryVM>> GetCustomerContractHistoryAsync(Guid customerId)
        {
            var contracts = await _unitOfWork.Contracts.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<CustomerContractSummaryVM>>(contracts) ?? new List<CustomerContractSummaryVM>();
        }

        public async Task<List<CustomerDetailVM>> GetAllAsync()
        {
            // Use the base repository's GetAllAsync method from IRepository<Customer>
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return _mapper.Map<List<CustomerDetailVM>>(customers) ?? new List<CustomerDetailVM>();
        }
    }

    public interface ICustomerService
    {
        Task<CustomerDetailVM> CreateAsync(CustomerCreateVM model);
        Task<CustomerDetailVM> UpdateAsync(CustomerUpdateVM model);
        Task<CustomerDetailVM> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<PagedResult<CustomerListVM>> GetPagedAsync(string? searchTerm, int page, int pageSize, string? orderBy = null);
        Task<CustomerDetailVM> UpdateContactInfoAsync(Guid id, string street, string city, string state, string zipCode, string country, string phoneCountryCode, string phoneAreaCode, string phoneNumber, string phoneType, string email);
        Task<CustomerDetailVM> UpdateIdentityAsync(Guid id, string firstName, string lastName, string nationalId);
        Task<List<CustomerContractSummaryVM>> GetCustomerContractsAsync(Guid customerId);
        Task<PagedResult<CustomerListVM>> SearchCustomersAsync(string term, int page, int pageSize);
        Task<PagedResult<CustomerListVM>> AdvancedSearchAsync(AdvancedCustomerSearchCriteriaVM criteria, int page, int pageSize);
        Task<CustomerAnalyticsVM> GetCustomerAnalyticsAsync(Guid? customerId = null);
        Task<CustomerAnalyticsVM> GetPortfolioAnalyticsAsync();
        Task<BulkUpdateResultVM> BulkUpdateCustomersAsync(BulkCustomerUpdateVM updateModel);
        Task<List<CustomerDetailVM>> BulkGetCustomersAsync(List<Guid> customerIds);
        Task<List<CustomerDetailVM>> GetAllAsync();
        Task<CustomerDetailVM> LinkContractToCustomerAsync(Guid customerId, Guid contractId);
        Task<CustomerDetailVM> TransferContractAsync(Guid contractId, Guid fromCustomerId, Guid toCustomerId);
        Task<List<CustomerContractSummaryVM>> GetCustomerContractHistoryAsync(Guid customerId);
    }
}