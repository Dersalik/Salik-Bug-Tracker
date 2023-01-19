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
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<ProjectForUpdateDTO, Project>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProjectName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.TargetEndDate, opt => opt.MapFrom(src => src.TargetEndDate))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.ActualEndDate))
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Module, ModuleDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.ActualEndDate))
                .ForMember(dest => dest.TargetEndDate, opt => opt.MapFrom(src => src.TargetEndDate))
                .ReverseMap();

            CreateMap<ModuleForCreationDTO, Module>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
              .ForMember(dest => dest.TargetEndDate, opt => opt.MapFrom(src => src.TargetEndDate))
              .ReverseMap();

            CreateMap<ModuleForUpdateDTO, Module>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Id, opt => opt.Ignore())
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
             .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
             .ForMember(dest => dest.TargetEndDate, opt => opt.MapFrom(src => src.TargetEndDate))
             .ReverseMap();

            CreateMap<ApplicationUser, UserDTO>().ForMember(dest=>dest.Id, opt=>opt.MapFrom(src=>src.Id))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.speciality, opt => opt.MapFrom(src => src.speciality))
             .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score)).ReverseMap();

            CreateMap<Skill, SkillDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Skill, SkillDTOForUpdate>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level)).ReverseMap();

            CreateMap<Skill, SkillDTOForCreation>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level)).ReverseMap();

        }
    }
}
