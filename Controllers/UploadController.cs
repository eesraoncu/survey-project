using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        try
        {
            _logger.LogInformation("Resim yükleme isteği alındı. Dosya adı: {FileName}, Boyut: {FileSize}", 
                file?.FileName, file?.Length);

            // Request detaylarını logla
            _logger.LogInformation("Content-Type: {ContentType}", Request.ContentType);
            _logger.LogInformation("Content-Length: {ContentLength}", Request.ContentLength);
            _logger.LogInformation("Form Files Count: {FormFilesCount}", Request.Form.Files.Count);

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Dosya boş veya null");
                return BadRequest(new { message = "Dosya seçilmedi" });
            }

            // Dosya türü kontrolü
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            _logger.LogInformation("Dosya uzantısı: {FileExtension}", fileExtension);
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                _logger.LogWarning("Geçersiz dosya türü: {FileExtension}", fileExtension);
                return BadRequest(new { message = "Sadece resim dosyaları kabul edilir" });
            }

            // Dosya boyutu kontrolü (5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning("Dosya boyutu çok büyük: {FileSize} bytes", file.Length);
                return BadRequest(new { message = "Dosya boyutu 5MB'dan küçük olmalıdır" });
            }

            // Uploads klasörünü oluştur
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                _logger.LogInformation("Uploads klasörü oluşturuldu: {UploadsFolder}", uploadsFolder);
            }

            // Benzersiz dosya adı oluştur
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Dosya başarıyla kaydedildi: {FilePath}", filePath);

            // URL oluştur
            var fileUrl = $"/uploads/{fileName}";
            
            return Ok(new { 
                message = "Resim başarıyla yüklendi",
                fileName = fileName,
                fileUrl = fileUrl,
                originalName = file.FileName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resim yükleme sırasında hata oluştu");
            return StatusCode(500, new { message = "Resim yüklenirken bir hata oluştu" });
        }
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        _logger.LogInformation("Upload test endpoint'i çağrıldı");
        return Ok(new { message = "Upload controller çalışıyor", timestamp = DateTime.UtcNow });
    }
}
