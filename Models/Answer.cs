using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Answer
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("answers_text")]
    public string AnswersText { get; set; } = string.Empty;

    [BsonElement("choice_ids")]
    public List<int> ChoiceIds { get; set; } = new List<int>(); // For multiple choice questions

    [BsonElement("selected_choices")]
    public List<string> SelectedChoices { get; set; } = new List<string>(); // Text of selected choices

    [BsonElement("numeric_value")]
    public double? NumericValue { get; set; } // For rating, scale, number questions

    [BsonElement("date_value")]
    public DateTime? DateValue { get; set; } // For date questions

    [BsonElement("boolean_value")]
    public bool? BooleanValue { get; set; } // For yes/no questions

    [BsonElement("questions_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int QuestionsId { get; set; } // Foreign Key

    [BsonElement("users_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UsersId { get; set; } // Foreign Key - Who answered

    [BsonElement("surveys_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SurveysId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Helper method to get the display value
    [BsonIgnore]
    public string DisplayValue
    {
        get
        {
            if (SelectedChoices.Any())
                return string.Join(", ", SelectedChoices);
            if (BooleanValue.HasValue)
                return BooleanValue.Value ? "Evet" : "Hayır";
            if (NumericValue.HasValue)
                return NumericValue.Value.ToString();
            if (DateValue.HasValue)
                return DateValue.Value.ToString("dd.MM.yyyy");
            return AnswersText;
        }
    }
} 