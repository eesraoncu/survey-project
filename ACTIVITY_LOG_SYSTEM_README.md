# Activity Log Sistemi

## Genel Bakış

Bu sistem, kullanıcı aktivitelerini detaylı bir şekilde loglamak için tasarlanmıştır. Kullanıcı girişleri, anket oluşturma, hata durumları ve diğer önemli işlemler otomatik olarak kaydedilir.

## Özellikler

### 📊 Loglanan Aktivite Tipleri
- **login_success**: Başarılı kullanıcı girişi
- **login_failed**: Başarısız giriş denemesi
- **survey_created**: Yeni anket oluşturma
- **survey_creation_failed**: Anket oluşturma hatası
- **survey_updated**: Anket güncelleme
- **survey_update_failed**: Anket güncelleme hatası
- **survey_deleted**: Anket silme
- **survey_delete_failed**: Anket silme hatası
- **survey_completed**: Anket tamamlama
- **survey_completion_failed**: Anket tamamlama hatası
- **question_created**: Soru oluşturma
- **answer_submitted**: Cevap gönderme

### 🔍 Log Detayları
Her log kaydı şu bilgileri içerir:
- **Kullanıcı ID**: İşlemi yapan kullanıcı
- **Aktivite Tipi**: Yapılan işlemin türü
- **Açıklama**: İşlem hakkında detaylı bilgi
- **IP Adresi**: Kullanıcının IP adresi
- **User Agent**: Tarayıcı bilgisi
- **Kaynak ID**: İlgili kaynağın ID'si (anket, soru vb.)
- **Kaynak Tipi**: Kaynağın türü
- **Ek Veriler**: JSON formatında ek bilgiler
- **Başarı Durumu**: İşlemin başarılı olup olmadığı
- **Hata Mesajı**: Hata varsa detayı
- **Zaman Damgası**: İşlem zamanı

## Veritabanı Yapısı

### ActivityLog Modeli
```csharp
public class ActivityLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ActivityType { get; set; }
    public string Description { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int? ResourceId { get; set; }
    public string? ResourceType { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
}
```

## API Endpoint'leri

### 1. Tüm Logları Getir
```http
GET /api/ActivityLog?page=1&pageSize=20
```

### 2. Kullanıcı Loglarını Getir
```http
GET /api/ActivityLog/user/{userId}?limit=50
```

### 3. Aktivite Tipine Göre Logları Getir
```http
GET /api/ActivityLog/type/{activityType}?limit=50
```

### 4. Tarih Aralığına Göre Logları Getir
```http
GET /api/ActivityLog/date-range?startDate=2024-01-01&endDate=2024-01-31
```

### 5. Log Sayısını Getir
```http
GET /api/ActivityLog/count
```

### 6. Log Sil
```http
DELETE /api/ActivityLog/{id}
```

## Kullanım Örnekleri

### Service Katmanında Log Ekleme
```csharp
// Başarılı işlem logu
await _activityLogService.LogActivityAsync(
    userId: user.Id,
    activityType: "survey_created",
    description: $"Yeni anket oluşturuldu: {surveyName}",
    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
    userAgent: Request.Headers["User-Agent"].ToString(),
    resourceId: surveyId,
    resourceType: "survey",
    additionalData: new Dictionary<string, object>
    {
        ["survey_name"] = surveyName,
        ["survey_type_id"] = surveyTypeId
    },
    isSuccessful: true
);

// Hata logu
await _activityLogService.LogActivityAsync(
    userId: user.Id,
    activityType: "survey_creation_failed",
    description: $"Anket oluşturma hatası: {surveyName}",
    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
    userAgent: Request.Headers["User-Agent"].ToString(),
    isSuccessful: false,
    errorMessage: ex.Message
);
```

### Controller'da Log Ekleme
```csharp
[HttpPost]
public async Task<ActionResult<SurveyResponse>> Create([FromBody] SurveyCreateRequest request)
{
    try
    {
        // İşlem kodları...
        
        // Başarılı log
        await _activityLogService.LogActivityAsync(
            userId: request.UsersId,
            activityType: "survey_created",
            description: $"Yeni anket oluşturuldu: {request.SurveyName}",
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
            userAgent: Request.Headers["User-Agent"].ToString(),
            resourceId: created.Id,
            resourceType: "survey",
            isSuccessful: true
        );
        
        return Ok(response);
    }
    catch (Exception ex)
    {
        // Hata logu
        await _activityLogService.LogActivityAsync(
            userId: request.UsersId,
            activityType: "survey_creation_failed",
            description: $"Anket oluşturma hatası: {request.SurveyName}",
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
            userAgent: Request.Headers["User-Agent"].ToString(),
            isSuccessful: false,
            errorMessage: ex.Message
        );
        
        throw;
    }
}
```

## Test Etme

### Test Sayfası
`http://localhost:5000/test-activity-logs.html` adresinde test sayfası bulunmaktadır.

### Manuel Test
```bash
# Son logları getir
curl http://localhost:5000/api/ActivityLog

# Başarılı girişleri getir
curl http://localhost:5000/api/ActivityLog/type/login_success

# Kullanıcı loglarını getir
curl http://localhost:5000/api/ActivityLog/user/1

# Log sayısını getir
curl http://localhost:5000/api/ActivityLog/count
```

## Güvenlik

### Log Güvenliği
- IP adresleri kaydedilir (güvenlik analizi için)
- User Agent bilgileri saklanır
- Hata detayları loglanır
- Başarısız işlemler ayrıca işaretlenir

### Veri Koruma
- Hassas bilgiler (şifreler) loglanmaz
- Sadece gerekli metadata kaydedilir
- Loglar belirli süre sonra arşivlenebilir

## Performans

### Optimizasyon
- Loglar asenkron olarak kaydedilir
- Batch işlemler için hazırlık yapılmıştır
- Index'ler otomatik oluşturulur
- Pagination desteği vardır

### Monitoring
- Log sayısı takip edilebilir
- Hata oranları analiz edilebilir
- Kullanıcı aktivite paternleri incelenebilir

## Gelecek Geliştirmeler

### Planlanan Özellikler
- [ ] Log arşivleme sistemi
- [ ] Real-time log streaming
- [ ] Log analitik dashboard'u
- [ ] Otomatik log temizleme
- [ ] Log export özelliği
- [ ] Email notification sistemi

### Entegrasyonlar
- [ ] Elasticsearch entegrasyonu
- [ ] Kibana dashboard'u
- [ ] Slack/Teams notification
- [ ] Grafana metrikleri

## Sorun Giderme

### Yaygın Sorunlar

#### 1. Log Kaydedilmiyor
- Service dependency injection kontrol edin
- MongoDB bağlantısını kontrol edin
- Exception handling'i kontrol edin

#### 2. Performans Sorunları
- Log sayısını kontrol edin
- Index'leri kontrol edin
- Batch işlemleri kullanın

#### 3. Veri Tutarsızlığı
- MongoDB bağlantı ayarlarını kontrol edin
- Transaction kullanımını kontrol edin

### Debug İpuçları
```csharp
// Debug için log ekleme
_logger.LogInformation("Activity log kaydediliyor: {ActivityType}", activityType);

// Log detaylarını kontrol etme
var log = await _activityLogService.GetRecentActivitiesAsync(1);
_logger.LogInformation("Son log: {@Log}", log.FirstOrDefault());
```

## Katkıda Bulunma

### Yeni Aktivite Tipi Ekleme
1. `ActivityLog` modelinde gerekli alanları ekleyin
2. Service katmanında log metodunu çağırın
3. API endpoint'ini güncelleyin
4. Test sayfasını güncelleyin
5. Dokümantasyonu güncelleyin

### Kod Standartları
- Async/await kullanın
- Exception handling yapın
- Log mesajlarını Türkçe yazın
- Unit test yazın
- Dokümantasyonu güncelleyin
