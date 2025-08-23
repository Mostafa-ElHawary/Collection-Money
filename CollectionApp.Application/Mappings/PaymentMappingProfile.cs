using System;
using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Application.Mappings
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<Payment, PaymentDetailVM>()
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : string.Empty))
                .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.Installment != null ? (int?)src.Installment.InstallmentNumber : null))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.ProcessedByStaffId))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<Payment, PaymentListVM>()
                .ForMember(dest => dest.ContractId, opt => opt.MapFrom(src => src.ContractId))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : string.Empty))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Contract != null && src.Contract.Customer != null ? $"{src.Contract.Customer.FirstName} {src.Contract.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.InstallmentId, opt => opt.MapFrom(src => src.InstallmentId))
                .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.Installment != null ? src.Installment.InstallmentNumber : (int?)null))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.ProcessedByStaffId, opt => opt.MapFrom(src => src.ProcessedByStaffId))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber));

            // ViewModel to Entity mappings
            CreateMap<PaymentCreateVM, Payment>()
                .ConstructUsing((src, context) => new Payment(
                    src.ContractId,
                    src.InstallmentId,
                    new Money(src.Amount, src.Currency),
                    src.PaymentDate,
                    src.PaymentMethod,
                    src.ReferenceNumber,
                    src.Notes,
                    src.ProcessedByStaffId));

            CreateMap<PaymentUpdateVM, Payment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ContractId, opt => opt.Ignore())
                .ForMember(dest => dest.InstallmentId, opt => opt.Ignore())
                .ForMember(dest => dest.Amount, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentDate, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
                .ForMember(dest => dest.ProcessedByStaffId, opt => opt.Ignore())
                .ForMember(dest => dest.StaffId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.Installment, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .ForMember(dest => dest.Receipt, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.UpdateDetails(src.ReferenceNumber, src.Notes);
                });

            CreateMap<CustomerListVM, CustomerSearchResultVM>();
            CreateMap<CustomerDetailVM, CustomerSearchResultVM>();
        }
    }
}

