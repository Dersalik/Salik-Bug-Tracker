using AutoMapper;
using Salik_Bug_Tracker_API.DTO;

namespace Salik_Bug_Tracker_API.Models.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Project,ProjectDTO>().ForMember(dest=>dest.ProjectName,opt=>opt.MapFrom(src=>src.Name))
                .ForMember(dest=>dest.Description,opt=>opt.MapFrom(src=>src.Description))
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.TargetEndDate, opt => opt.MapFrom(src => src.TargetEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.ActualEndDate))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();


            CreateMap<ProjectForCreationDTO, Project>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.TargetEndDate, opt => opt.MapFrom(src => src.TargetEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.ActualEndDate))
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<ProjectForUpdateDTO, Project>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProjectName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.TargetEndDate, opt => opt.MapFrom(src => src.TargetEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.ActualEndDate))
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();


        }
    }
}
