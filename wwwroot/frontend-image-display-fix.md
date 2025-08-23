# Frontend'de Resim Görüntüleme Sorunu Çözümü

## 🔍 Sorun:
Backend'den gelen resim URL'i `/uploads/fileName.jpg` şeklinde ama frontend'de resim görüntülenmiyor.

## 🔧 Çözüm:

### 1. **Resim URL'ini Doğru Şekilde Oluşturma:**

```typescript
// ❌ YANLIŞ - Sadece relative path
const imageUrl = survey.surveyBackgroundImage; // "/uploads/fileName.jpg"

// ✅ DOĞRU - Tam URL oluşturma
const imageUrl = `http://localhost:5000${survey.surveyBackgroundImage}`;
// veya
const imageUrl = `${import.meta.env.VITE_API_BASE_URL}${survey.surveyBackgroundImage}`;
```

### 2. **Forms.tsx'de Resim Görüntüleme:**

```typescript
// Forms.tsx
const Forms: React.FC = () => {
  const [survey, setSurvey] = useState<Survey | null>(null);
  
  // Resim URL'ini oluştur
  const getImageUrl = (imagePath: string | undefined) => {
    if (!imagePath) return '';
    // Eğer zaten tam URL ise
    if (imagePath.startsWith('http')) return imagePath;
    // Relative path ise tam URL oluştur
    return `http://localhost:5000${imagePath}`;
  };

  return (
    <div 
      className="survey-container"
      style={{
        backgroundImage: survey?.surveyBackgroundImage 
          ? `url(${getImageUrl(survey.surveyBackgroundImage)})`
          : 'none',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        backgroundRepeat: 'no-repeat',
        minHeight: '100vh'
      }}
    >
      {/* Anket içeriği */}
      {survey?.surveyBackgroundImage && (
        <div className="background-image-info">
          <p>Arka plan resmi: {survey.surveyBackgroundImage}</p>
          <img 
            src={getImageUrl(survey.surveyBackgroundImage)}
            alt="Arka plan önizleme"
            style={{ maxWidth: '200px', maxHeight: '200px' }}
          />
        </div>
      )}
    </div>
  );
};
```

### 3. **Environment Variable Kullanımı:**

```typescript
// .env dosyasında:
VITE_API_BASE_URL=http://localhost:5000

// Kodda:
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const getImageUrl = (imagePath: string | undefined) => {
  if (!imagePath) return '';
  if (imagePath.startsWith('http')) return imagePath;
  return `${API_BASE_URL}${imagePath}`;
};
```

### 4. **CORS Sorunu Çözümü:**

Eğer CORS hatası alıyorsanız, backend'de Program.cs'de CORS ayarlarını kontrol edin:

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### 5. **Test Etmek İçin:**

1. **Backend'de test endpoint'ini kontrol edin:**
   ```
   http://localhost:5000/api/upload/test
   ```

2. **Yüklenen resmi doğrudan kontrol edin:**
   ```
   http://localhost:5000/uploads/125a5b68-f120-4913-81f7-87c2e7a60aef.jpg
   ```

3. **Frontend'de console'da URL'i kontrol edin:**
   ```javascript
   console.log('Resim URL:', getImageUrl(survey.surveyBackgroundImage));
   ```

### 6. **Hata Ayıklama:**

```typescript
// Resim yükleme hatası kontrolü
const handleImageError = (event: React.SyntheticEvent<HTMLImageElement, Event>) => {
  console.error('Resim yüklenemedi:', event.currentTarget.src);
  // Fallback resim göster
  event.currentTarget.src = '/fallback-image.jpg';
};

<img 
  src={getImageUrl(survey.surveyBackgroundImage)}
  alt="Arka plan"
  onError={handleImageError}
/>
```

## 🎯 Özet:

**Sorun**: Frontend'de resim URL'i doğru oluşturulmuyor
**Çözüm**: Backend URL'ini (http://localhost:5000) resim path'ine ekleyin
**Sonuç**: Resimler doğru şekilde görüntülenecek

Bu değişiklikleri yaptıktan sonra Forms.tsx sayfasında resimler görünecektir.
