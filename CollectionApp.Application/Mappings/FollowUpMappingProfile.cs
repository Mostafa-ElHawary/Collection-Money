using System;
using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Application.Mappings
{
    public class FollowUpMappingProfile : Profile
    {
        public FollowUpMappingProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<FollowUp, FollowUpDetailVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : string.Empty))
                .ForMember(dest => dest.AssignedStaffName, opt => opt.MapFrom(src => src.AssignedToStaffId.HasValue && src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}" : string.Empty))
                .ForMember(dest => dest.AssignedToStaffId, opt => opt.MapFrom(src => src.AssignedToStaffId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src => src.ScheduledDate))
                .ForMember(dest => dest.ActualDate, opt => opt.MapFrom(src => src.ActualDate))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Outcome, opt => opt.MapFrom(src => src.Outcome));

            CreateMap<FollowUp, FollowUpListVM>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : string.Empty))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : string.Empty))
                .ForMember(dest => dest.AssignedToStaffId, opt => opt.MapFrom(src => src.AssignedToStaffId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src => src.ScheduledDate));

            // ViewModel to Entity mappings
            CreateMap<FollowUpCreateVM, FollowUp>()
                .ConstructUsing((src, context) => new FollowUp(
                    src.CustomerId,
                    src.ContractId,
                    src.AssignedToStaffId,
                    src.Type,
                    src.Priority,
                    src.Description,
                    src.ScheduledDate,
                    src.Notes));

            CreateMap<FollowUpUpdateVM, FollowUp>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.ContractId, opt => opt.Ignore())
                .ForMember(dest => dest.StaffId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.AssignedToStaffId.HasValue && src.AssignedToStaffId != dest.AssignedToStaffId)
                    {
                        dest.AssignToStaff(src.AssignedToStaffId.Value);
                    }
                    
                    dest.UpdateDetails(
                        src.Type,
                        src.Priority,
                        src.Description,
                        src.ScheduledDate,
                        src.Notes);
                    
                    if (!string.IsNullOrEmpty(src.Status) && src.Status != dest.Status)
                    {
                        dest.UpdateStatus(src.Status);
                    }
                })
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.UpdateDetails(
                        src.Type,
                        src.Priority,
                        src.Description,
                        src.ScheduledDate,
                        src.Notes);

                    if (src.AssignedToStaffId.HasValue) dest.AssignToStaff(src.AssignedToStaffId.Value);
                    if (!string.IsNullOrWhiteSpace(src.Status)) dest.UpdateStatus(src.Status);
                });
        }
    }
}

