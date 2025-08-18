using AutoMapper;
using SurveyApp.Models;
using SurveyApp.Application.DTO.Request;
using SurveyApp.Application.DTO.Response;

namespace SurveyApp.Application.Profiles;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Survey
        CreateMap<SurveyCreateRequest, Survey>();
        CreateMap<SurveyUpdateRequest, Survey>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<Survey, SurveyResponse>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));
        CreateMap<Survey, SurveyListItemResponse>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()))
            .ForMember(d => d.UsersId, o => o.MapFrom(s => s.UsersId));

        // Question
        CreateMap<QuestionCreateRequest, Question>()
            // Frontend'den gelebilecek alternatif alan adlarını normalize et
            .ForMember(d => d.QuestionsText, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.QuestionsText) ? (s.QuestionText ?? string.Empty) : s.QuestionsText))
            .ForMember(d => d.SurveysId, o => o.MapFrom(s => s.SurveysId > 0 ? s.SurveysId : (s.SurveyId ?? 0)));
        CreateMap<QuestionUpdateRequest, Question>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.SurveysId, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<Question, QuestionResponse>();

        // Choice
        CreateMap<ChoiceCreateRequest, Choice>();
        CreateMap<ChoiceUpdateRequest, Choice>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.QuestionsId, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<Choice, ChoiceResponse>();

        // Answer
        CreateMap<AnswerCreateRequest, Answer>();
        CreateMap<AnswerUpdateRequest, Answer>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.QuestionsId, o => o.Ignore())
            .ForMember(d => d.UsersId, o => o.Ignore())
            .ForMember(d => d.SurveysId, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<Answer, AnswerResponse>();

        // User
        CreateMap<UserRegisterRequest, User>();
        CreateMap<UserUpdateRequest, User>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserEmail, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.RoleName).ToList()));

        // Event
        CreateMap<EventRequest, Event>();
        CreateMap<EventUpdateRequest, Event>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<Event, EventResponse>();

        // UserSettings
        CreateMap<UserSettingsRequest, UserSettings>();
        CreateMap<UserSettingsUpdateRequest, UserSettings>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<UserSettings, UserSettingsResponse>();

        // UserProfile
        CreateMap<UserProfileRequest, UserProfile>();
        CreateMap<UserProfileUpdateRequest, UserProfile>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore());
        CreateMap<UserProfile, UserProfileResponse>();
    }
}


