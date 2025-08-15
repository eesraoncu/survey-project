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
        CreateMap<Survey, SurveyResponse>();
        CreateMap<Survey, SurveyListItemResponse>();

        // Question
        CreateMap<QuestionCreateRequest, Question>();
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
    }
}


