namespace SurveyApp.Application.DTO.Request;

public class SurveyDeleteRequest
{
    public int UsersId { get; set; } // Silme işlemini yapan kullanıcının ID'si
    public string? Reason { get; set; } // Silme nedeni (opsiyonel)
}
