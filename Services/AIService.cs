using Microsoft.Extensions.Options;
using SurveyApp.Models;
using SurveyApp.Infrastructure.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using AutoMapper;

namespace SurveyApp.Services;

public class AIService : IAIService
{
    private readonly GoogleAISettings _aiSettings;
    private readonly HttpClient _httpClient;
    private readonly ISurveyRepository _surveyRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly AutoIncrementService _autoIncrementService;
    private readonly IMapper _mapper;
    
    // Repository'lere public erişim için
    public ISurveyRepository GetSurveyRepository() => _surveyRepository;
    public IQuestionRepository GetQuestionRepository() => _questionRepository;

    public AIService(
        IOptions<GoogleAISettings> aiSettings,
        HttpClient httpClient,
        ISurveyRepository surveyRepository,
        IAnswerRepository answerRepository,
        IQuestionRepository questionRepository,
        AutoIncrementService autoIncrementService,
        IMapper mapper)
    {
        _aiSettings = aiSettings.Value;
        _httpClient = httpClient;
        _surveyRepository = surveyRepository;
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
        _autoIncrementService = autoIncrementService;
        _mapper = mapper;
    }

    public async Task<AIAnalysisResult> AnalyzeSurveyResultsAsync(int surveyId, string analysisType)
    {
        try
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null)
                throw new ArgumentException("Survey not found");

            var answers = await _answerRepository.GetBySurveyIdAsync(surveyId);
            var questions = await _questionRepository.GetBySurveyIdAsync(surveyId);

            var prompt = BuildAnalysisPrompt(survey, questions.ToList(), answers.ToList(), analysisType);
            var aiResponse = await CallGoogleAIAsync(prompt);

            var analysisResult = new AIAnalysisResult
            {
                SurveyId = surveyId,
                AnalysisType = analysisType,
                AnalysisResult = aiResponse,
                ConfidenceScore = 0.85, // Default confidence
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1 // System user
            };

            return analysisResult;
        }
        catch (Exception ex)
        {
            throw new Exception($"AI analysis failed: {ex.Message}");
        }
    }

    public async Task<SentimentAnalysis> AnalyzeSentimentAsync(List<string> responses)
    {
        try
        {
            var prompt = $@"
            Analyze the sentiment of the following survey responses and provide a detailed sentiment analysis in Turkish.
            
            Responses:
            {string.Join("\n", responses.Select((r, i) => $"{i + 1}. {r}"))}
            
            Please provide:
            1. Overall sentiment (positive, negative, neutral)
            2. Percentage breakdown
            3. Key positive themes (max 5)
            4. Key negative themes (max 5)
            
            Format the response as JSON with these fields:
            - overall_sentiment
            - positive_percentage
            - negative_percentage  
            - neutral_percentage
            - key_positive_themes (array)
            - key_negative_themes (array)
            ";

            var aiResponse = await CallGoogleAIAsync(prompt);
            return ParseSentimentAnalysis(aiResponse);
        }
        catch (Exception ex)
        {
            throw new Exception($"Sentiment analysis failed: {ex.Message}");
        }
    }

    public async Task<List<SurveyInsight>> GenerateInsightsAsync(int surveyId)
    {
        try
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null)
                throw new ArgumentException("Survey not found");

            var answers = await _answerRepository.GetBySurveyIdAsync(surveyId);
            var questions = await _questionRepository.GetBySurveyIdAsync(surveyId);

            var prompt = $@"
            Generate actionable insights for this survey in Turkish:
            
            Survey: {survey.SurveyName}
            Description: {survey.SurveyDescription}
            
            Questions and Answers:
            {BuildQuestionsAndAnswersText(questions.ToList(), answers.ToList())}
            
            Please provide 5-10 actionable insights with categories and importance levels.
            Format as JSON array with fields: category, insight, importance (high/medium/low)
            ";

            var aiResponse = await CallGoogleAIAsync(prompt);
            return ParseInsights(aiResponse);
        }
        catch (Exception ex)
        {
            throw new Exception($"Insight generation failed: {ex.Message}");
        }
    }

    public async Task<string> GenerateSummaryAsync(int surveyId)
    {
        try
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null)
                throw new ArgumentException("Survey not found");

            var answers = await _answerRepository.GetBySurveyIdAsync(surveyId);
            var questions = await _questionRepository.GetBySurveyIdAsync(surveyId);

            var prompt = $@"
            Create a comprehensive summary of this survey in Turkish:
            
            Survey: {survey.SurveyName}
            Description: {survey.SurveyDescription}
            Total Responses: {answers.Count()}
            
            Questions and Answers:
            {BuildQuestionsAndAnswersText(questions.ToList(), answers.ToList())}
            
            Please provide:
            1. Executive summary (2-3 paragraphs)
            2. Key findings (bullet points)
            3. Participant demographics insights
            4. Recommendations for next steps
            
            Write in a professional report format.
            ";

            return await CallGoogleAIAsync(prompt);
        }
        catch (Exception ex)
        {
            throw new Exception($"Summary generation failed: {ex.Message}");
        }
    }

    public async Task<List<SmartQuestion>> GenerateSmartQuestionsAsync(QuestionSuggestionRequest request)
    {
        try
        {
            var prompt = $@"
            Generate {request.NumberOfSuggestions} smart survey questions in Turkish for:
            
            Survey Title: {request.SurveyTitle}
            Description: {request.SurveyDescription}
            Category: {request.Category}
            
            Existing Questions (avoid duplicates):
            {string.Join("\n", request.ExistingQuestions.Select((q, i) => $"{i + 1}. {q}"))}
            
            For each question, provide:
            1. Question text
            2. Question type (multiple_choice, text, rating, yes_no)
            3. Suggested choices (if applicable)
            4. Category
            5. Confidence score (0-1)
            
            Format as JSON array with fields: question_text, question_type, suggested_choices, category, ai_confidence
            ";

            var aiResponse = await CallGoogleAIAsync(prompt);
            return ParseSmartQuestions(aiResponse);
        }
        catch (Exception ex)
        {
            throw new Exception($"Smart question generation failed: {ex.Message}");
        }
    }

    public async Task<List<string>> SuggestAnswerChoicesAsync(string questionText, string questionType)
    {
        try
        {
            if (questionType != "multiple_choice" && questionType != "rating")
                return new List<string>();

            var prompt = $@"
            Generate appropriate answer choices in Turkish for this question:
            
            Question: {questionText}
            Type: {questionType}
            
            Provide 4-6 relevant choices that cover the range of possible responses.
            Return as a simple JSON array of strings.
            ";

            var aiResponse = await CallGoogleAIAsync(prompt);
            return ParseAnswerChoices(aiResponse);
        }
        catch (Exception ex)
        {
            throw new Exception($"Answer choice suggestion failed: {ex.Message}");
        }
    }

    public async Task<string> ImproveQuestionTextAsync(string questionText)
    {
        try
        {
            var prompt = $@"
            Improve this survey question to make it clearer, more engaging, and bias-free:
            
            Original Question: {questionText}
            
            Please provide:
            1. Improved version
            2. Brief explanation of changes made
            
            Write in Turkish and maintain the original intent.
            ";

            return await CallGoogleAIAsync(prompt);
        }
        catch (Exception ex)
        {
            throw new Exception($"Question improvement failed: {ex.Message}");
        }
    }

    public async Task<List<string>> ExtractKeywordsAsync(string text)
    {
        try
        {
            var prompt = $@"
            Extract the most important keywords and phrases from this text:
            
            Text: {text}
            
            Return 10-15 relevant keywords/phrases in Turkish.
            Format as a simple JSON array of strings.
            ";

            var aiResponse = await CallGoogleAIAsync(prompt);
            return ParseKeywords(aiResponse);
        }
        catch (Exception ex)
        {
            throw new Exception($"Keyword extraction failed: {ex.Message}");
        }
    }

    public async Task<string> TranslateTextAsync(string text, string targetLanguage)
    {
        try
        {
            var prompt = $@"
            Translate the following text to {targetLanguage}:
            
            Text: {text}
            
            Provide only the translation, maintaining the original tone and meaning.
            ";

            return await CallGoogleAIAsync(prompt);
        }
        catch (Exception ex)
        {
            throw new Exception($"Translation failed: {ex.Message}");
        }
    }

    public async Task<string> GenerateReportAsync(int surveyId, string reportType)
    {
        try
        {
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null)
                throw new ArgumentException("Survey not found");

            var answers = await _answerRepository.GetBySurveyIdAsync(surveyId);
            var questions = await _questionRepository.GetBySurveyIdAsync(surveyId);

            var prompt = $@"
            Generate a {reportType} report in Turkish for this survey:
            
            Survey: {survey.SurveyName}
            Description: {survey.SurveyDescription}
            
            Questions and Answers:
            {BuildQuestionsAndAnswersText(questions.ToList(), answers.ToList())}
            
            Report Type: {reportType}
            
            Create a professional, detailed report with charts suggestions, conclusions, and recommendations.
            ";

            return await CallGoogleAIAsync(prompt);
        }
        catch (Exception ex)
        {
            throw new Exception($"Report generation failed: {ex.Message}");
        }
    }

    public async Task<Survey> GenerateCompleteSurveyAsync(string description, int userId)
    {
        try
        {
            // AI'dan anket bilgilerini ve soruları al
            var prompt = $@"
            Create a complete survey in Turkish based on this description: '{description}'
            
            Generate:
            1. Survey title (maximum 100 characters)
            2. Survey description (maximum 500 characters)
            3. 5-10 relevant questions with their types and choices
            
            Format the response as JSON with this structure:
            {{
                ""survey_title"": ""title"",
                ""survey_description"": ""description"",
                ""questions"": [
                    {{
                        ""question_text"": ""question"",
                        ""question_type"": ""multiple_choice/text/rating/yes_no"",
                        ""choices"": [""choice1"", ""choice2"", ""choice3"", ""choice4""]
                    }}
                ]
            }}
            
            Make questions engaging, clear, and relevant to the topic.
            For multiple_choice questions, provide 3-5 meaningful options.
            For rating questions, use a 1-5 or 1-10 scale.
            ";

            var aiResponse = await CallGoogleAIAsync(prompt);
            Console.WriteLine($"AI Response: {aiResponse}");
            
            var surveyData = ParseSurveyGenerationResponse(aiResponse);
            Console.WriteLine($"Parsed Survey Data: Title={surveyData.SurveyTitle}, Questions Count={surveyData.Questions.Count}");

            // Survey oluştur (ID ataması repository tarafından yapılacak)
            var survey = new Survey
            {
                SurveyName = surveyData.SurveyTitle,
                SurveyDescription = surveyData.SurveyDescription,
                UsersId = userId,
                SurveyTypeId = 1, // Default survey type
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsCompleted = false
            };

            // Survey'i kaydet
            var createdSurvey = await _surveyRepository.CreateAsync(survey);
            
            Console.WriteLine($"✓ Survey created with ID: {createdSurvey.Id}");
            Console.WriteLine($"✓ Questions created: {surveyData.Questions.Count}");

            // Soruları oluştur
            foreach (var questionData in surveyData.Questions)
            {
                var questionId = await _autoIncrementService.GetNextIdAsync("questions");
                var question = new Question
                {
                    Id = questionId,
                    QuestionsText = questionData.QuestionText,
                    QuestionType = questionData.QuestionType,
                    Choices = questionData.Choices ?? new List<string>(),
                    SurveysId = createdSurvey.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await _questionRepository.CreateAsync(question);

                // Eğer multiple choice sorusu ise seçenekleri oluştur
                if (questionData.QuestionType == "multiple_choice" && questionData.Choices != null)
                {
                    // Choice repository'si olmadığı için şimdilik sadece soru metninde belirtelim
                    // Gelecekte Choice model'i eklendiğinde burası genişletilebilir
                }
            }

            return createdSurvey;
        }
        catch (Exception ex)
        {
            throw new Exception($"Complete survey generation failed: {ex.Message}");
        }
    }

    private async Task<string> CallGoogleAIAsync(string prompt)
    {
        try
        {
            Console.WriteLine($"=== Google AI API Call Starting ===");
            Console.WriteLine($"API Key: {_aiSettings.ApiKey?.Substring(0, 10)}...");
            Console.WriteLine($"Model: {_aiSettings.Model}");
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = _aiSettings.Temperature,
                    maxOutputTokens = _aiSettings.MaxTokens
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_aiSettings.Model}:generateContent?key={_aiSettings.ApiKey}";
            Console.WriteLine($"Request URL: {url.Replace(_aiSettings.ApiKey, "***")}");
            Console.WriteLine($"Request Body: {json}");
            
            var response = await _httpClient.PostAsync(url, content);
            
            Console.WriteLine($"Response Status: {response.StatusCode}");
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Google AI API error: {response.StatusCode} - {responseContent}");
            }

            // JSON deserialization sorununu bypass edelim ve direct text extract yapalım
            try
            {
                var result = JsonSerializer.Deserialize<GoogleAIResponse>(responseContent);
                var aiText = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
                
                if (!string.IsNullOrEmpty(aiText))
                {
                    Console.WriteLine($"Extracted AI Text via Deserialize: {aiText.Substring(0, Math.Min(100, aiText.Length))}...");
                    Console.WriteLine($"=== Google AI API Call Completed ===");
                    return aiText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization failed: {ex.Message}");
            }
            
            // Fallback: Direct text extraction from JSON
            Console.WriteLine("Using fallback text extraction...");
            
            // JSON'dan text alanını daha güvenli şekilde extract et
            var textStart = responseContent.IndexOf("\"text\": \"");
            if (textStart != -1)
            {
                textStart += 9; // "text": " length
                var textEnd = responseContent.LastIndexOf("\"");
                
                if (textEnd > textStart)
                {
                    var extractedText = responseContent.Substring(textStart, textEnd - textStart);
                    // JSON escape karakterlerini temizle
                    extractedText = extractedText.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
                    
                    Console.WriteLine($"Extracted AI Text via String Method: {extractedText.Substring(0, Math.Min(100, extractedText.Length))}...");
                    Console.WriteLine($"=== Google AI API Call Completed ===");
                    return extractedText;
                }
            }
            
            Console.WriteLine($"Could not extract text, returning full response");
            return responseContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Google AI API call failed: {ex.Message}");
            throw new Exception($"Google AI API call failed: {ex.Message}");
        }
    }

    private string BuildAnalysisPrompt(Survey survey, List<Question> questions, List<Answer> answers, string analysisType)
    {
        var prompt = $@"
        Analyze this survey data for {analysisType} analysis in Turkish:
        
        Survey: {survey.SurveyName}
        Description: {survey.SurveyDescription}
        Total Responses: {answers.Count}
        
        Questions and Answers:
        {BuildQuestionsAndAnswersText(questions, answers)}
        
        Please provide a detailed {analysisType} analysis with actionable insights.
        ";
        
        return prompt;
    }

    private string BuildQuestionsAndAnswersText(List<Question> questions, List<Answer> answers)
    {
        var result = new StringBuilder();
        
        foreach (var question in questions)
        {
            result.AppendLine($"Question: {question.QuestionsText}");
            var questionAnswers = answers.Where(a => a.QuestionsId == question.Id).ToList();
            
            foreach (var answer in questionAnswers)
            {
                result.AppendLine($"  - {answer.AnswersText}");
            }
            result.AppendLine();
        }
        
        return result.ToString();
    }

    private SentimentAnalysis ParseSentimentAnalysis(string aiResponse)
    {
        try
        {
            return JsonSerializer.Deserialize<SentimentAnalysis>(ExtractJsonFromResponse(aiResponse)) ?? new SentimentAnalysis();
        }
        catch
        {
            return new SentimentAnalysis
            {
                OverallSentiment = "neutral",
                PositivePercentage = 33.3,
                NegativePercentage = 33.3,
                NeutralPercentage = 33.3
            };
        }
    }

    private List<SurveyInsight> ParseInsights(string aiResponse)
    {
        try
        {
            return JsonSerializer.Deserialize<List<SurveyInsight>>(ExtractJsonFromResponse(aiResponse)) ?? new List<SurveyInsight>();
        }
        catch
        {
            return new List<SurveyInsight>();
        }
    }

    private List<SmartQuestion> ParseSmartQuestions(string aiResponse)
    {
        try
        {
            return JsonSerializer.Deserialize<List<SmartQuestion>>(ExtractJsonFromResponse(aiResponse)) ?? new List<SmartQuestion>();
        }
        catch
        {
            return new List<SmartQuestion>();
        }
    }

    private List<string> ParseAnswerChoices(string aiResponse)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(ExtractJsonFromResponse(aiResponse)) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private List<string> ParseKeywords(string aiResponse)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(ExtractJsonFromResponse(aiResponse)) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private string ExtractJsonFromResponse(string response)
    {
        Console.WriteLine($"ExtractJsonFromResponse Input: {response.Substring(0, Math.Min(100, response.Length))}...");
        
        // Extract JSON from AI response that might contain markdown code blocks
        var cleanResponse = response.Trim();
        
        // ```json ... ``` formatını temizle
        if (cleanResponse.StartsWith("```json"))
        {
            var lines = cleanResponse.Split('\n');
            var jsonLines = new List<string>();
            bool insideJson = false;
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == "```json")
                {
                    insideJson = true;
                    continue;
                }
                if (lines[i].Trim() == "```")
                {
                    break;
                }
                if (insideJson)
                {
                    jsonLines.Add(lines[i]);
                }
            }
            
            cleanResponse = string.Join('\n', jsonLines);
        }
        
        // ``` ile başlayıp bitiyor mu kontrol et (json etiketi olmadan)
        else if (cleanResponse.StartsWith("```") && cleanResponse.EndsWith("```"))
        {
            var firstNewline = cleanResponse.IndexOf('\n');
            var lastBacktick = cleanResponse.LastIndexOf("```");
            
            if (firstNewline != -1 && lastBacktick > firstNewline)
            {
                cleanResponse = cleanResponse.Substring(firstNewline + 1, lastBacktick - firstNewline - 1);
            }
        }
        
        cleanResponse = cleanResponse.Trim();
        Console.WriteLine($"ExtractJsonFromResponse Output: {cleanResponse.Substring(0, Math.Min(100, cleanResponse.Length))}...");
        
        return cleanResponse;
    }

    private SurveyGenerationData ParseSurveyGenerationResponse(string aiResponse)
    {
        try
        {
            var cleanJson = ExtractJsonFromResponse(aiResponse);
            Console.WriteLine($"Extracted JSON: {cleanJson}");
            
            var result = JsonSerializer.Deserialize<SurveyGenerationData>(cleanJson);
            
            if (result == null)
            {
                Console.WriteLine("Deserialization returned null, using fallback");
                return GetFallbackSurveyData();
            }
            
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parse error: {ex.Message}");
            Console.WriteLine($"Raw AI Response: {aiResponse}");
            
            // Fallback değerler
            return GetFallbackSurveyData();
        }
    }
    
    private SurveyGenerationData GetFallbackSurveyData()
    {
        return new SurveyGenerationData
        {
            SurveyTitle = "AI Destekli Müşteri Memnuniyeti Anketi",
            SurveyDescription = "Bu anket AI tarafından oluşturulmuştur ve müşteri memnuniyetini ölçmeyi amaçlar.",
            Questions = new List<SurveyQuestionData>
            {
                new SurveyQuestionData
                {
                    QuestionText = "Ürün/hizmetimizden genel olarak ne kadar memnunsunuz?",
                    QuestionType = "multiple_choice",
                    Choices = new List<string> { "Çok memnunum", "Memnunum", "Kararsızım", "Memnun değilim", "Hiç memnun değilim" }
                },
                new SurveyQuestionData
                {
                    QuestionText = "Hangi konularda iyileştirme yapabiliriz?",
                    QuestionType = "text",
                    Choices = new List<string>()
                },
                new SurveyQuestionData
                {
                    QuestionText = "Bizi arkadaşlarınıza tavsiye eder misiniz?",
                    QuestionType = "yes_no",
                    Choices = new List<string> { "Evet", "Hayır" }
                }
            }
        };
    }
}

// Survey Generation Models
public class SurveyGenerationData
{
    [JsonPropertyName("survey_title")]
    public string SurveyTitle { get; set; } = string.Empty;
    
    [JsonPropertyName("survey_description")]
    public string SurveyDescription { get; set; } = string.Empty;
    
    [JsonPropertyName("questions")]
    public List<SurveyQuestionData> Questions { get; set; } = new List<SurveyQuestionData>();
}

public class SurveyQuestionData
{
    [JsonPropertyName("question_text")]
    public string QuestionText { get; set; } = string.Empty;
    
    [JsonPropertyName("question_type")]
    public string QuestionType { get; set; } = string.Empty;
    
    [JsonPropertyName("choices")]
    public List<string>? Choices { get; set; }
}

// Google AI API Response Models
public class GoogleAIResponse
{
    public GoogleAICandidate[]? Candidates { get; set; }
}

public class GoogleAICandidate
{
    public GoogleAIContent? Content { get; set; }
}

public class GoogleAIContent
{
    public GoogleAIPart[]? Parts { get; set; }
}

public class GoogleAIPart
{
    public string? Text { get; set; }
}
