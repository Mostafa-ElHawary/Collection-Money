using System;
using System.Linq;
using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;
using System.Collections.Generic;

namespace CollectionApp.Application.Mappings
{
    public class StaffMappingProfile : Profile
    {
        public StaffMappingProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<Staff, StaffDetailVM>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.PhoneCountryCode, opt => opt.MapFrom(src => src.Phone.CountryCode))
                .ForMember(dest => dest.PhoneAreaCode, opt => opt.MapFrom(src => src.Phone.AreaCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone.Number))
                .ForMember(dest => dest.PhoneType, opt => opt.MapFrom(src => src.Phone.PhoneType))
                .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Salary))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<Staff, StaffListVM>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.PhoneCountryCode, opt => opt.MapFrom(src => src.Phone.CountryCode))
                .ForMember(dest => dest.PhoneAreaCode, opt => opt.MapFrom(src => src.Phone.AreaCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone.Number))
                .ForMember(dest => dest.PhoneType, opt => opt.MapFrom(src => src.Phone.PhoneType))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department));

            // ViewModel to Entity mappings
            CreateMap<StaffCreateVM, Staff>()
                .ConstructUsing((src, context) => new Staff(
                    src.EmployeeId,
                    src.FirstName,
                    src.LastName,
                    src.Email,
                    new Phone($"+{src.PhoneCountryCode}{src.PhoneAreaCode}{src.PhoneNumber}", src.PhoneType),
                    src.Position,
                    src.Department,
                    src.HireDate,
                    src.Salary,
                    src.IsActive,
                    src.Permissions,
                    src.Notes));

            CreateMap<StaffUpdateVM, Staff>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .ForMember(dest => dest.FollowUps, opt => opt.Ignore())
                .ForMember(dest => dest.Visits, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.UpdatePersonalInfo(
                        src.FirstName ?? dest.FirstName, 
                        src.LastName ?? dest.LastName, 
                        src.Position ?? dest.Position, 
                        src.Department ?? dest.Department);

                    dest.UpdateContactInfo(
                        src.Email ?? dest.Email, 
                        new Phone($"+{src.PhoneCountryCode}{src.PhoneAreaCode}{src.PhoneNumber}", src.PhoneType));

                    dest.UpdateEmploymentInfo(
                        src.HireDate ?? dest.HireDate, 
                        src.Salary, 
                        src.IsActive);

                    dest.UpdatePermissions(src.Permissions ?? new List<string>());

                    dest.UpdateNotes(src.Notes);
                });

            // Related entities -> StaffRelatedSummaryVM
            CreateMap<Payment, StaffRelatedSummaryVM>()
                .ForMember(d => d.Reference, opt => opt.MapFrom(s => !string.IsNullOrWhiteSpace(s.ReferenceNumber)
                    ? s.ReferenceNumber!
                    : (s.Contract != null ? s.Contract.ContractNumber : "Payment")))
                .ForMember(d => d.Date, opt => opt.MapFrom(s => s.PaymentDate))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => $"{s.PaymentMethod} {s.Amount.Currency} {s.Amount.Amount:N2}"));

            CreateMap<Receipt, StaffRelatedSummaryVM>()
                .ForMember(d => d.Reference, opt => opt.MapFrom(s => s.ReceiptNumber))
                .ForMember(d => d.Date, opt => opt.MapFrom(s => s.IssueDate))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description ?? $"Receipt {s.ReceiptNumber}"));

            CreateMap<Visit, StaffRelatedSummaryVM>()
                .ForMember(d => d.Reference, opt => opt.MapFrom(_ => "Visit"))
                .ForMember(d => d.Date, opt => opt.MapFrom(s => s.VisitDate))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Purpose));

            CreateMap<FollowUp, StaffRelatedSummaryVM>()
                .ForMember(d => d.Reference, opt => opt.MapFrom(s => s.Contract != null ? s.Contract.ContractNumber : "FollowUp"))
                .ForMember(d => d.Date, opt => opt.MapFrom(s => s.ActualDate ?? s.ScheduledDate))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => $"{s.Type} - {s.Status}"));
        }
    }
}

