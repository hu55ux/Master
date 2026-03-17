using AutoMapper;
using Master.Application.DTOs;
using Master.Application.Models;

namespace Master.Application.Mapping
{
    /// <summary>
    /// Defines AutoMapper mappings between DTOs and domain models.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Configures mappings for AutoMapper.
        /// </summary>
        public MappingProfile()
        {
            // User registration mapping
            CreateMap<RegisterRequest, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src =>
                    src.DateOfBirth.ToDateTime(TimeOnly.MinValue)))
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore());

            // AppUser -> AuthResponseDTO mapping
            CreateMap<AppUser, AuthResponseDTO>()
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            // Profile editing mapping
            CreateMap<ProfileEditRequest, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTimeOffset.UtcNow));

            // JobPost creation mapping
            CreateMap<CreateJobPostDTO, JobPost>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            // JobPost update mapping (ignores nulls)
            CreateMap<UpdateJobPostDTO, JobPost>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // JobPost -> JobPostResponseDTO mapping
            CreateMap<JobPost, JobPostResponseDTO>()
                .ForMember(dest => dest.RequiredSkillId, opt => opt.MapFrom(src => src.RequiredSkillId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.JPStatus.ToString()))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer!.UserName))
                .ForMember(dest => dest.RequiredSkillName, opt => opt.MapFrom(src => src.RequiredSkill!.Name));

            // Skill -> SkillResponseDTO mapping
            CreateMap<Skill, SkillResponseDTO>();

            // Skill creation mapping
            CreateMap<CreateSkillDTO, Skill>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.UserSkills, opt => opt.Ignore())
                .ForMember(dest => dest.JobPosts, opt => opt.Ignore());

            // Skill update mapping (ignores nulls)
            CreateMap<UpdateSkillDTO, Skill>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}