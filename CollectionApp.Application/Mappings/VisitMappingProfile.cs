using System;
using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Application.Mappings
{
    public class VisitMappingProfile : Profile
    {
        public VisitMappingProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<Visit, VisitDetailVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty))
                .ForMember(dest => dest.VisitType, opt => opt.MapFrom(src => src.VisitType))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.DurationHours, opt => opt.MapFrom(src => src.Duration.HasValue ? (int?)src.Duration.Value.Hours : null))
                .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.Duration.HasValue ? (int?)src.Duration.Value.Minutes : null))
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Purpose))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<Visit, VisitListVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty))
                .ForMember(dest => dest.VisitType, opt => opt.MapFrom(src => src.VisitType))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.DurationHours, opt => opt.MapFrom(src => src.Duration.HasValue ? (int?)src.Duration.Value.Hours : null))
                .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.Duration.HasValue ? (int?)src.Duration.Value.Minutes : null))
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Purpose));

            // ViewModel to Entity mappings
            CreateMap<VisitCreateVM, Visit>()
                .ConstructUsing((src, context) => new Visit(
                    src.CustomerId,
                    src.StaffId,
                    src.VisitDate,
                    src.DurationHours > 0 || src.DurationMinutes > 0 ? 
                        TimeSpan.FromHours((double)src.DurationHours).Add(TimeSpan.FromMinutes((double)src.DurationMinutes)) : null,
                    src.VisitType,
                    src.Location,
                    src.Purpose,
                    src.Notes));

            CreateMap<VisitUpdateVM, Visit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.StaffId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    TimeSpan? duration = null;
                    if ((src.DurationHours ?? 0) > 0 || (src.DurationMinutes ?? 0) > 0)
                    {
                        duration = TimeSpan.FromHours(src.DurationHours ?? 0).Add(TimeSpan.FromMinutes(src.DurationMinutes ?? 0));
                    }
                    
                    dest.UpdateDetails(
                        src.VisitDate.HasValue ? src.VisitDate.Value : dest.VisitDate,
                        duration,
                        src.VisitType,
                        src.Location,
                        src.Purpose,
                        src.Notes);
                })
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    var hours = src.DurationHours ?? 0;
                    var minutes = src.DurationMinutes ?? 0;
                    var duration = src.Duration ?? new TimeSpan(hours, minutes, 0);
                    dest.UpdateDetails(
                        src.VisitDate.HasValue ? src.VisitDate.Value : dest.VisitDate,
                        duration,
                        src.VisitType,
                        src.Location,
                        src.Purpose,
                        src.Notes);
                });
        }
    }
}

