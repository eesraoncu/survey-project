using SurveyApp.Models;

namespace SurveyApp.Services;

public interface IAIService
{
    // Survey Analysis
    Task<AIAnalysisResult> AnalyzeSurveyResultsAsync(int surveyId, string analysisType);
    Task<SentimentAnalysis> AnalyzeSentimentAsync(List<string> responses);
    Task<List<SurveyInsight>> GenerateInsightsAsync(int surveyId);
    Task<string> GenerateSummaryAsync(int surveyId);

    // Smart Question Generation
    Task<List<SmartQuestion>> GenerateSmartQuestionsAsync(QuestionSuggestionRequest request);
    Task<List<string>> SuggestAnswerChoicesAsync(string questionText, string questionType);

    // Text Processing
    Task<string> ImproveQuestionTextAsync(string questionText);
    Task<List<string>> ExtractKeywordsAsync(string text);
    Task<string> TranslateTextAsync(string text, string targetLanguage);

    // Report Generation
    Task<string> GenerateReportAsync(int surveyId, string reportType);

    // Complete Survey Generation
    Task<Survey> GenerateCompleteSurveyAsync(string description, int userId);
}
