using AutoMapper;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Service.Features.Tasks.Commands;
using TaskManagement.Service.Models.Dtos;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Service.Mappings;

/// <summary>
/// AutoMapper profile for Task mappings.
/// </summary>
public class TaskMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskMappingProfile"/> class.
    /// </summary>
    public TaskMappingProfile()
    {
        // Entity to DTO
        CreateMap<ManagementTask, TaskDto>();

        // Command to Entity (for create)
        CreateMap<CreateTaskCommand, ManagementTask>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TaskStatus.NotStarted))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // DTO to Entity (for API layer - if needed)
        CreateMap<CreateTaskDto, ManagementTask>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TaskStatus.NotStarted))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}

