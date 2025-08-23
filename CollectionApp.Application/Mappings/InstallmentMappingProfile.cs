using System;
using System.Linq;
using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;
// Remove unused using since System.Linq.Dynamic.Core is not referenced in the project

namespace CollectionApp.Application.Mappings
{
    public class InstallmentMappingProfile : Profile
    {
        public InstallmentMappingProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<Installment, InstallmentDetailVM>()
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : string.Empty))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.InstallmentNumber))
                .ForMember(dest => dest.PaidDate, opt => opt.MapFrom(src => src.PaidDate));

            CreateMap<Installment, InstallmentListVM>()
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : string.Empty))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.InstallmentNumber))
                .ForMember(dest => dest.PaidDate, opt => opt.MapFrom(src => src.PaidDate));

            // ViewModel to Entity mappings
            CreateMap<InstallmentCreateVM, Installment>()
                .ConstructUsing((src, context) => new Installment(
                    src.ContractId,
                    src.InstallmentNumber,
                    src.DueDate,
                    new Money(src.Amount, src.Currency)));

            CreateMap<InstallmentUpdateVM, Installment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ContractId, opt => opt.Ignore())
                .ForMember(dest => dest.InstallmentNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .AfterMap((src, dest) =>
                {
                    dest.UpdateDetails(
                        new Money(src.Amount, src.Currency),
                        src.DueDate,
                        src.Notes);

                    if (src.Status != dest.Status)
                    {
                        dest.UpdateStatus(src.Status);
                    }

                    if (src.PaymentDate.HasValue)
                    {
                        dest.MarkAsPaid(src.PaymentDate.Value);
                    }
                });
        }
    }
}

