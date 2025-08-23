using System;
using System.Linq;
using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Application.Mappings
{
    public class ContractMappingProfile : Profile
    {
        public ContractMappingProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<Contract, ContractDetailVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.PrincipalAmount, opt => opt.MapFrom(src => src.PrincipalAmount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => "USD"))
                .ForMember(dest => dest.InterestRate, opt => opt.MapFrom(src => src.InterestRate))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.ContractType))
                .ForMember(dest => dest.TermInMonths, opt => opt.MapFrom(src => src.TermInMonths))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.PaymentFrequency, opt => opt.MapFrom(src => src.PaymentFrequency))
                .ForMember(dest => dest.GracePeriodDays, opt => opt.MapFrom(src => src.GracePeriodDays))
                .ForMember(dest => dest.LateFeePercentage, opt => opt.MapFrom(src => src.LateFeePercentage))
                .ForMember(dest => dest.PenaltyPercentage, opt => opt.MapFrom(src => src.PenaltyPercentage))
                .ForMember(dest => dest.CollateralDescription, opt => opt.MapFrom(src => src.CollateralDescription))
                .ForMember(dest => dest.GuarantorName, opt => opt.MapFrom(src => src.GuarantorName))
                .ForMember(dest => dest.GuarantorContact, opt => opt.MapFrom(src => src.GuarantorContact))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty));
                
            // Detail -> Update view model mapping for edit scenarios
            CreateMap<ContractDetailVM, ContractUpdateVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName));

            CreateMap<Contract, ContractListVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.PrincipalAmount, opt => opt.MapFrom(src => src.PrincipalAmount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => "USD"))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.ContractType))
                .ForMember(dest => dest.TermInMonths, opt => opt.MapFrom(src => src.TermInMonths))
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty));

            // Installment -> Summary for contract
            CreateMap<Installment, ContractInstallmentSummaryVM>()
                .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
                .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Amount.Currency));

            // Payment -> Summary for contract
            CreateMap<Payment, ContractPaymentSummaryVM>()
                .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
                .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Amount.Currency))
                .ForMember(d => d.PaymentDate, opt => opt.MapFrom(s => s.PaymentDate));

            // ===== New mappings for financial operations =====
            CreateMap<ContractInstallmentSummaryVM, PaymentModalVM>()
                .ForMember(d => d.InstallmentId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.InstallmentNumber, opt => opt.MapFrom(s => s.InstallmentNumber))
                .ForMember(d => d.InstallmentAmount, opt => opt.MapFrom(s => s.Amount))
                .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount - s.PaidAmount))
                .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Currency))
                .ForMember(d => d.PaymentDate, opt => opt.MapFrom(_ => DateTime.UtcNow.Date));

            CreateMap<Installment, WaiveInstallmentVM>()
                .ForMember(d => d.InstallmentId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.InstallmentNumber, opt => opt.MapFrom(s => s.InstallmentNumber))
                .ForMember(d => d.InstallmentAmount, opt => opt.MapFrom(s => s.Amount.Amount));

            CreateMap<Installment, RescheduleInstallmentVM>()
                .ForMember(d => d.InstallmentId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.InstallmentNumber, opt => opt.MapFrom(s => s.InstallmentNumber))
                .ForMember(d => d.CurrentDueDate, opt => opt.MapFrom(s => s.DueDate))
                .ForMember(d => d.NewDueDate, opt => opt.MapFrom(s => s.DueDate.AddDays(1)));

            // Map analytics summary from service to ContractFinancialSummaryVM
            CreateMap<InstallmentAnalyticsViewModels.InstallmentStatusSummaryVM, ContractFinancialSummaryVM>();

            // ViewModel to Entity mappings
            CreateMap<ContractCreateVM, Contract>()
                .ConstructUsing((src, context) => new Contract(
                    src.ContractNumber,
                    src.CustomerId,
                    src.ContractType,
                    new Money(src.PrincipalAmount, src.Currency),
                    new Money(src.InterestRate, src.Currency),
                    src.TermInMonths,
                    src.StartDate,
                    src.EndDate,
                    src.PaymentFrequency,
                    src.GracePeriodDays,
                    src.LateFeePercentage,
                    src.PenaltyPercentage,
                    src.CollateralDescription,
                    src.GuarantorName,
                    src.GuarantorContact,
                    src.Notes,
                    src.StaffId));

            CreateMap<ContractUpdateVM, Contract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ContractNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.StaffId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .ForMember(dest => dest.Installments, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ForMember(dest => dest.FollowUps, opt => opt.Ignore())
                .ForMember(dest => dest.Visits, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.UpdateTerms(
                        new Money(src.PrincipalAmount, src.Currency),
                        new Money(src.InterestRate, src.Currency),
                        src.TermInMonths,
                        src.StartDate,
                        src.EndDate,
                        src.PaymentFrequency,
                        src.GracePeriodDays,
                        src.LateFeePercentage,
                        src.PenaltyPercentage,
                        src.CollateralDescription,
                        src.GuarantorName,
                        src.GuarantorContact,
                        src.Notes,
                        src.StaffId);
                });
        }
    }
}

