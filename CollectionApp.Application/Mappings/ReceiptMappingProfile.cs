using System;
using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Application.Mappings
{
    public class ReceiptMappingProfile : Profile
    {
        public ReceiptMappingProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<Receipt, ReceiptDetailVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
                .ForMember(dest => dest.ReceiptNumber, opt => opt.MapFrom(src => src.ReceiptNumber))
                .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.Payment != null ? src.Payment.PaymentDate : (DateTime?)null))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty));

            CreateMap<Receipt, ReceiptListVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
                .ForMember(dest => dest.ReceiptNumber, opt => opt.MapFrom(src => src.ReceiptNumber))
                .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty));

            // ViewModel to Entity mappings
            CreateMap<ReceiptCreateVM, Receipt>()
                .ConstructUsing((src, context) => new Receipt(
                    src.ReceiptNumber,
                    src.PaymentId,
                    src.CustomerId,
                    new Money(src.Amount, src.Currency),
                    src.IssueDate,
                    src.Description,
                    src.StaffId));

            CreateMap<ReceiptUpdateVM, Receipt>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiptNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.Amount, opt => opt.Ignore())
                .ForMember(dest => dest.IssueDate, opt => opt.Ignore())
                .ForMember(dest => dest.StaffId, opt => opt.Ignore())
                .ForMember(dest => dest.Payment, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.Description != null)
                        dest.UpdateDescription(src.Description);
                });
        }
    }
}

