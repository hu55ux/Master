using AutoMapper;
using Master.Application.DTOs;
using Master.Domain.Models;

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
                .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => src.AverageRating))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.UserSkills.Select(us => us.Skill)));

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
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.UserName : null))
                .ForMember(dest => dest.RequiredSkillName, opt => opt.MapFrom(src => src.RequiredSkill != null ? src.RequiredSkill.Name : null))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl)));

            // Skill -> SkillResponseDTO mapping
            CreateMap<Skill, SkillResponseDTO>();

            // JobOffer -> JobOfferResponseDTO mapping
            CreateMap<JobOffer, JobOfferResponseDTO>()
                .ForMember(dest => dest.JobPostTitle, opt => opt.MapFrom(src => src.JobPost != null ? src.JobPost.Title : string.Empty))
                .ForMember(dest => dest.MasterFirstName, opt => opt.MapFrom(src => src.Master != null ? src.Master.FirstName : string.Empty))
                .ForMember(dest => dest.MasterLastName, opt => opt.MapFrom(src => src.Master != null ? src.Master.LastName : string.Empty))
                .ForMember(dest => dest.MasterProfileImageUrl, opt => opt.MapFrom(src => src.Master != null ? src.Master.ProfileImageUrl : null))
                .ForMember(dest => dest.MasterRating, opt => opt.MapFrom(src => src.Master != null ? src.Master.AverageRating : 0))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FirstName : string.Empty))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.LastName : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Skill creation mapping
            CreateMap<CreateSkillDTO, Skill>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.UserSkills, opt => opt.Ignore())
                .ForMember(dest => dest.JobPosts, opt => opt.Ignore());

            // Skill update mapping (ignores nulls)
            CreateMap<UpdateSkillDTO, Skill>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // CreateDTO -> Entity mapping
            CreateMap<CreateMasterRatingDTO, MasterRating>()
                // Əgər ClientId DTO-da CustomerId kimi gəlirsə, onu map edirik
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId));

            // UpdateDTO -> Entity mapping
            CreateMap<UpdateMasterRatingDTO, MasterRating>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Entity -> ResponseDTO mapping
            CreateMap<MasterRating, MasterRatingResponseDTO>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                // Ustanın adı (FirstName + LastName birləşməsini tövsiyə edirəm)
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src =>
                    src.Master != null ? $"{src.Master.FirstName} {src.Master.LastName}" : "Unknown Master"))
                // Müştərinin adı
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : "Unknown Customer"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}