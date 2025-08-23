using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SurveyApp.Models;

public class Survey
{
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }

    [BsonElement("survey_name")]
    public string SurveyName { get; set; } = string.Empty;

    [BsonElement("survey_description")]
    public string SurveyDescription { get; set; } = string.Empty;

    [BsonElement("users_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int UsersId { get; set; } // Foreign Key - Survey creator (will become owner)

    [BsonElement("survey_type_id")]
    [BsonRepresentation(BsonType.Int32)]
    public int SurveyTypeId { get; set; } // Foreign Key

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;

    [BsonElement("is_completed")]
    public bool IsCompleted { get; set; } = false; // Survey completion status

    // Anket özellikleri (2. görseldeki özellikler)
    [BsonElement("allows_anonymous_responses")]
    public bool AllowsAnonymousResponses { get; set; } = true;

    [BsonElement("requires_login")]
    public bool RequiresLogin { get; set; } = false;

    [BsonElement("max_responses")]
    public int? MaxResponses { get; set; } = null; // null = sınırsız

    [BsonElement("expires_at")]
    public DateTime? ExpiresAt { get; set; } = null; // null = süresiz

    [BsonElement("allows_editing")]
    public bool AllowsEditing { get; set; } = false; // Cevapları düzenleme izni

    [BsonElement("shows_results")]
    public bool ShowsResults { get; set; } = false; // Sonuçları gösterme

    // Derecelendirme ayarları (görselden - Derecelendirme)
    [BsonElement("enables_rating")]
    public bool EnablesRating { get; set; } = false; // Derecelendirme özelliği aktif mi
    
    [BsonElement("rating_scale_max")]
    public int RatingScaleMax { get; set; } = 5; // Derecelendirme için maksimum değer (1-5, 1-10, vs.)

    [BsonElement("rating_scale_min")]
    public int RatingScaleMin { get; set; } = 1; // Derecelendirme için minimum değer

    // Tarih özelliği (görselden - Tarih)
    [BsonElement("enables_date_fields")]
    public bool EnablesDateFields { get; set; } = false; // Tarih alanları aktif mi

    [BsonElement("date_format")]
    public string DateFormat { get; set; } = "dd/MM/yyyy"; // Tarih formatı

    // Konum özelliği (görselden - Konum)
    [BsonElement("enables_location")]
    public bool EnablesLocation { get; set; } = false; // Konum toplama aktif mi

    [BsonElement("location_precision")]
    public string LocationPrecision { get; set; } = "city"; // "exact", "city", "country"

    // Telefon özelliği (görselden - Telefon)
    [BsonElement("enables_phone")]
    public bool EnablesPhone { get; set; } = false; // Telefon numarası toplama aktif mi

    [BsonElement("phone_format")]
    public string PhoneFormat { get; set; } = "TR"; // Telefon formatı (TR, US, vb.)

    // E-posta özelliği (görselden - E-posta)
    [BsonElement("enables_email")]
    public bool EnablesEmail { get; set; } = false; // E-posta toplama aktif mi

    [BsonElement("email_verification")]
    public bool EmailVerification { get; set; } = false; // E-posta doğrulama gerekli mi

    // Ad Soyad özelliği (görselden - Ad Soyad)
    [BsonElement("enables_full_name")]
    public bool EnablesFullName { get; set; } = false; // Ad soyad toplama aktif mi

    [BsonElement("name_format")]
    public string NameFormat { get; set; } = "first_last"; // "first_last", "last_first", "single_field"

    // Arka plan resmi özelliği
    [BsonElement("survey_background_image")]
    public string? SurveyBackgroundImage { get; set; } = null; // Arka plan resmi URL'i
} 