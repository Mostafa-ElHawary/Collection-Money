using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Application.Mappings;

public class LedgerEntryMappingProfile : Profile
{
    public LedgerEntryMappingProfile()
    {
        // Entity to ViewModel mappings
        CreateMap<LedgerEntry, LedgerEntryDetailVM>()
            .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : string.Empty))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
            .ForMember(dest => dest.DebitAmount, opt => opt.MapFrom(src => src.DebitAmount.Amount))
            .ForMember(dest => dest.CreditAmount, opt => opt.MapFrom(src => src.CreditAmount.Amount))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.DebitAmount.Currency))
            .ForMember(dest => dest.ReferenceType, opt => opt.MapFrom(src => src.ReferenceType))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
            .ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.ReferenceId))
            .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.StaffId))
            .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty));

        CreateMap<LedgerEntry, LedgerEntryListVM>()
            .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : string.Empty))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
            .ForMember(dest => dest.DebitAmount, opt => opt.MapFrom(src => src.DebitAmount.Amount))
            .ForMember(dest => dest.CreditAmount, opt => opt.MapFrom(src => src.CreditAmount.Amount))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.DebitAmount.Currency))
            .ForMember(dest => dest.ReferenceType, opt => opt.MapFrom(src => src.ReferenceType))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate));

        // ViewModel to Entity mappings
        CreateMap<LedgerEntryCreateVM, LedgerEntry>()
            .ConstructUsing((src, context) => new LedgerEntry(
                src.TransactionDate,
                src.Description,
                new Money(src.DebitAmount, src.Currency),
                new Money(src.CreditAmount, src.Currency),
                new Money(src.Balance, src.Currency),
                src.ReferenceType,
                src.ReferenceId,
                src.ContractId,
                src.CustomerId,
                src.StaffId));

        CreateMap<LedgerEntryUpdateVM, LedgerEntry>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ContractId, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.TransactionDate, opt => opt.Ignore())
            .ForMember(dest => dest.DebitAmount, opt => opt.Ignore())
            .ForMember(dest => dest.CreditAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Balance, opt => opt.Ignore())
            .ForMember(dest => dest.ReferenceType, opt => opt.Ignore())
            .ForMember(dest => dest.ReferenceId, opt => opt.Ignore())
            .ForMember(dest => dest.StaffId, opt => opt.Ignore())
            .ForMember(dest => dest.Contract, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Staff, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}

