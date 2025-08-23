using System;
using System.Linq;
using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Application.Mappings
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<Customer, CustomerDetailVM>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Address.State))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address.Country))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.Address.PostalCode))
                .ForMember(dest => dest.PhoneCountryCode, opt => opt.MapFrom(src => src.Phone.CountryCode))
                .ForMember(dest => dest.PhoneAreaCode, opt => opt.MapFrom(src => src.Phone.AreaCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone.Number))
                .ForMember(dest => dest.PhoneType, opt => opt.MapFrom(src => src.Phone.PhoneType))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
                .ForMember(dest => dest.EmployerName, opt => opt.MapFrom(src => src.EmployerName))
                .ForMember(dest => dest.MonthlyIncome, opt => opt.MapFrom(src => src.MonthlyIncome))
                .ForMember(dest => dest.CreditScore, opt => opt.MapFrom(src => src.CreditScore))
                .ForMember(dest => dest.SourceOfFunds, opt => opt.MapFrom(src => src.SourceOfFunds))
                .ForMember(dest => dest.PurposeOfLoan, opt => opt.MapFrom(src => src.PurposeOfLoan))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<Customer, CustomerListVM>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Address.State))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Address.Country))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.Address.PostalCode))
                .ForMember(dest => dest.PhoneCountryCode, opt => opt.MapFrom(src => src.Phone.CountryCode))
                .ForMember(dest => dest.PhoneAreaCode, opt => opt.MapFrom(src => src.Phone.AreaCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone.Number))
                .ForMember(dest => dest.PhoneType, opt => opt.MapFrom(src => src.Phone.PhoneType))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
                .ForMember(dest => dest.ActiveContractsCount, opt => opt.MapFrom(src => src.Contracts.Count(c => c.Status == ContractStatus.Active)))
                .ForMember(dest => dest.TotalContractValue, opt => opt.MapFrom(src => src.Contracts.Sum(c => c.TotalAmount.Amount)));

            // Detail -> Update view model mapping for edit scenarios
            CreateMap<CustomerDetailVM, CustomerUpdateVM>();

            // ViewModel to Entity mappings
            CreateMap<CustomerCreateVM, Customer>()
                .ConstructUsing((src, context) => new Customer(
                    src.FirstName,
                    src.LastName,
                    src.NationalId,
                    new Address(src.Street, src.City, src.State, src.Country, src.ZipCode),
                    new Phone($"+{src.PhoneCountryCode}{src.PhoneAreaCode}{src.PhoneNumber}", src.PhoneType),
                    src.Email,
                    src.DateOfBirth,
                    src.Gender,
                    src.Occupation,
                    src.EmployerName,
                    src.MonthlyIncome,
                    src.CreditScore,
                    src.SourceOfFunds,
                    src.PurposeOfLoan,
                    src.Notes));

            CreateMap<CustomerUpdateVM, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.UpdateIdentity(
                        src.FirstName,
                        src.LastName,
                        src.NationalId);

                    dest.UpdatePersonalInfo(
                        src.DateOfBirth,
                        src.Gender,
                        src.Occupation,
                        src.EmployerName,
                        src.MonthlyIncome,
                        src.CreditScore,
                        src.SourceOfFunds,
                        src.PurposeOfLoan,
                        src.Notes);

                    dest.UpdateContact(
                        new Address(src.Street, src.City, src.State, src.Country, src.ZipCode),
                        new Phone($"+{src.PhoneCountryCode}{src.PhoneAreaCode}{src.PhoneNumber}", src.PhoneType),
                        src.Email);
                });

            // Contract -> CustomerContractSummaryVM mapping
            CreateMap<Contract, CustomerContractSummaryVM>()
                .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.TotalAmount.Amount))
                .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.TotalAmount.Currency))
                .ForMember(d => d.StartDate, opt => opt.MapFrom(s => s.StartDate))
                .ForMember(d => d.EndDate, opt => opt.MapFrom(s => s.EndDate))
                .ForMember(d => d.PaidAmount, opt => opt.MapFrom(s => s.Installments.Sum(i => i.PaidAmount.Amount)))
                .ForMember(d => d.NextPaymentDueDate, opt => opt.Ignore());

            // Enhanced computed properties for lists/details can be added via AfterMap when needed

            // Map for analytics (entity -> analytics VM), computed fields typically filled in service
            CreateMap<Customer, CustomerAnalyticsVM>()
                .ForMember(d => d.TotalCustomers, opt => opt.Ignore())
                .ForMember(d => d.MaleCustomers, opt => opt.Ignore())
                .ForMember(d => d.FemaleCustomers, opt => opt.Ignore())
                .ForMember(d => d.OtherGenderCustomers, opt => opt.Ignore())
                .ForMember(d => d.AverageMonthlyIncome, opt => opt.Ignore())
                .ForMember(d => d.TotalPortfolioValue, opt => opt.Ignore())
                .ForMember(d => d.ActiveContracts, opt => opt.Ignore())
                .ForMember(d => d.CompletedContracts, opt => opt.Ignore())
                .ForMember(d => d.DefaultedContracts, opt => opt.Ignore())
                .ForMember(d => d.CustomersByCountry, opt => opt.Ignore())
                .ForMember(d => d.CustomersByState, opt => opt.Ignore())
                .ForMember(d => d.CustomersByCity, opt => opt.Ignore())
                .ForMember(d => d.NewCustomersOverTime, opt => opt.Ignore())
                .ForMember(d => d.PortfolioValueOverTime, opt => opt.Ignore());

            // Map for search result highlighting (Customer -> CustomerSearchResultVM)
            CreateMap<Customer, CustomerSearchResultVM>()
                .IncludeBase<Customer, CustomerListVM>()
                .ForMember(d => d.Highlight, opt => opt.Ignore())
                .ForMember(d => d.MatchScore, opt => opt.Ignore());

            // Map bulk update VM to entity changes through AfterMap, actual persistence occurs in service
            CreateMap<BulkCustomerUpdateVM, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore())
                .ForMember(dest => dest.NationalId, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore())
                .ForMember(dest => dest.Gender, opt => opt.Ignore())
                .ForMember(dest => dest.Occupation, opt => opt.Ignore())
                .ForMember(dest => dest.EmployerName, opt => opt.Ignore())
                .ForMember(dest => dest.MonthlyIncome, opt => opt.Ignore())
                .ForMember(dest => dest.CreditScore, opt => opt.Ignore())
                .ForMember(dest => dest.SourceOfFunds, opt => opt.Ignore())
                .ForMember(dest => dest.PurposeOfLoan, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Phone, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    var street = src.Street ?? dest.Address.Street;
                    var city = src.City ?? dest.Address.City;
                    var state = src.State ?? dest.Address.State;
                    var country = src.Country ?? dest.Address.Country;
                    var zip = src.ZipCode ?? dest.Address.PostalCode;

                    dest.UpdateContact(
                        new Address(street, city, state, country, zip),
                        new Phone(
                            $"+{(src.PhoneCountryCode ?? dest.Phone.CountryCode)}{(src.PhoneAreaCode ?? dest.Phone.AreaCode)}{(src.PhoneNumber ?? dest.Phone.Number)}",
                            src.PhoneType ?? dest.Phone.PhoneType
                        ),
                        src.Email ?? dest.Email
                    );

                    if (!string.IsNullOrWhiteSpace(src.Notes))
                    {
                        dest.UpdatePersonalInfo(
                            dest.DateOfBirth,
                            dest.Gender,
                            dest.Occupation,
                            dest.EmployerName,
                            dest.MonthlyIncome,
                            dest.CreditScore,
                            dest.SourceOfFunds,
                            dest.PurposeOfLoan,
                            src.Notes
                        );
                    }
                });

            CreateMap<CustomerListVM, CustomerSearchResultVM>();
            CreateMap<IReadOnlyList<CustomerListVM>, List<CustomerSearchResultVM>>();
     
        CreateMap<IEnumerable<CustomerListVM>, List<CustomerSearchResultVM>>();
        CreateMap<IReadOnlyList<CustomerListVM>, List<CustomerSearchResultVM>>();
        }
    }
}

