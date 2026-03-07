using AutoMapper;
using Master.DTOs;
using Master.Models;
namespace Master.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, AppUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<AppUser, AuthResponseDTO>()
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokenExpiresAt, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        CreateMap<ProfileEditRequest, AppUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));

        CreateMap<CreateJobPostDTO, JobPost>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateJobPostDTO, JobPost>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<JobPost, JobPostResponseDTO>()
            .ForMember(dest => dest.RequiredSkillId, opt => opt.MapFrom(src => src.RequiredSkillId))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.JPStatus.ToString()))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.UserName))
            .ForMember(dest => dest.RequiredSkillName, opt => opt.MapFrom(src => src.RequiredSkill.Name));

        CreateMap<Skill, SkillResponseDTO>();

        CreateMap<CreateSkillDTO, Skill>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UserSkills, opt => opt.Ignore())
            .ForMember(dest => dest.JobPosts, opt => opt.Ignore());

        CreateMap<UpdateSkillDTO, Skill>();
    }
}
