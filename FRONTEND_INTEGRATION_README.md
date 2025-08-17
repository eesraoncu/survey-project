# Frontend Entegrasyonu - Yeni Özellikler

Bu dokümanda, frontend'de eklenen yeni özellikler için backend API endpoint'leri ve kullanım örnekleri bulunmaktadır.

## 🔐 Kimlik Doğrulama

Tüm yeni endpoint'ler JWT token gerektirir. Header'da `Authorization: Bearer {token}` şeklinde gönderilmelidir.

## 📅 Takvim (Events) API

### Endpoint'ler

#### 1. Tüm Etkinlikleri Getir
```http
GET /api/events
Authorization: Bearer {token}
```

#### 2. Belirli Bir Etkinliği Getir
```http
GET /api/events/{id}
Authorization: Bearer {token}
```

#### 3. Kullanıcının Etkinliklerini Getir
```http
GET /api/events/user/{userId}
Authorization: Bearer {token}
```

#### 4. Tarih Aralığına Göre Etkinlikleri Getir
```http
GET /api/events/date-range?startDate={date}&endDate={date}
Authorization: Bearer {token}
```

#### 5. Yeni Etkinlik Ekle
```http
POST /api/events
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Toplantı",
  "description": "Haftalık ekip toplantısı",
  "eventDate": "2024-01-15T00:00:00",
  "startTime": "09:00:00",
  "endTime": "10:00:00",
  "isAllDay": false,
  "location": "Konferans Salonu",
  "color": "#3788d8"
}
```

#### 6. Etkinlik Güncelle
```http
PUT /api/events/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "title": "Güncellenmiş Toplantı",
  "description": "Güncellenmiş açıklama",
  "eventDate": "2024-01-15T00:00:00",
  "startTime": "09:00:00",
  "endTime": "10:00:00",
  "isAllDay": false,
  "location": "Konferans Salonu",
  "color": "#3788d8"
}
```

#### 7. Etkinlik Sil
```http
DELETE /api/events/{id}
Authorization: Bearer {token}
```

## ⚙️ Kullanıcı Ayarları (UserSettings) API

### Endpoint'ler

#### 1. Kullanıcı Ayarlarını Getir
```http
GET /api/usersettings
Authorization: Bearer {token}
```

#### 2. Kullanıcı Ayarlarını Oluştur
```http
POST /api/usersettings
Authorization: Bearer {token}
Content-Type: application/json

{
  "profilePicture": "https://example.com/avatar.jpg",
  "bio": "Merhaba, ben bir geliştiriciyim",
  "phoneNumber": "+90 555 123 4567",
  "website": "https://example.com",
  "emailNotifications": true,
  "pushNotifications": true,
  "weeklyReports": false,
  "surveyReminders": true,
  "dataAnalytics": true,
  "thirdPartyIntegrations": false,
  "profileVisibility": "public",
  "theme": "light",
  "colorScheme": "default",
  "fontSize": "medium",
  "language": "tr",
  "dateFormat": "dd/MM/yyyy",
  "timeFormat": "24",
  "twoFactorEnabled": false,
  "loginNotifications": true,
  "sessionTimeout": 30
}
```

#### 3. Kullanıcı Ayarlarını Güncelle
```http
PUT /api/usersettings
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "theme": "dark",
  "colorScheme": "blue",
  "language": "en"
}
```

#### 4. Kullanıcı Ayarlarını Sil
```http
DELETE /api/usersettings/{id}
Authorization: Bearer {token}
```

## 👤 Kullanıcı Profili (UserProfile) API

### Endpoint'ler

#### 1. Kullanıcı Profilini Getir
```http
GET /api/userprofile
Authorization: Bearer {token}
```

#### 2. Kullanıcı Profili Oluştur
```http
POST /api/userprofile
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "email": "ahmet@example.com",
  "phoneNumber": "+90 555 123 4567",
  "profilePicture": "https://example.com/avatar.jpg",
  "bio": "Merhaba, ben bir geliştiriciyim",
  "dateOfBirth": "1990-01-01T00:00:00",
  "gender": "male",
  "location": "İstanbul, Türkiye",
  "website": "https://example.com",
  "socialMedia": {
    "linkedin": "https://linkedin.com/in/ahmet",
    "twitter": "https://twitter.com/ahmet",
    "github": "https://github.com/ahmet"
  },
  "interests": ["Yazılım Geliştirme", "Müzik", "Spor"],
  "skills": ["C#", "JavaScript", "React", "Node.js"],
  "education": [
    {
      "institution": "İstanbul Teknik Üniversitesi",
      "degree": "Lisans",
      "fieldOfStudy": "Bilgisayar Mühendisliği",
      "startDate": "2008-09-01T00:00:00",
      "endDate": "2012-06-01T00:00:00",
      "description": "Bilgisayar mühendisliği lisans programı"
    }
  ],
  "experience": [
    {
      "company": "Tech Company",
      "position": "Senior Developer",
      "startDate": "2020-01-01T00:00:00",
      "endDate": null,
      "description": "Full-stack geliştirici olarak çalışıyorum",
      "isCurrent": true
    }
  ]
}
```

#### 3. Kullanıcı Profilini Güncelle
```http
PUT /api/userprofile
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "bio": "Güncellenmiş bio"
}
```

#### 4. Kullanıcı Profilini Sil
```http
DELETE /api/userprofile/{id}
Authorization: Bearer {token}
```

## 🔒 Şifre Değiştirme API

### Endpoint

#### Şifre Değiştir
```http
PUT /api/users/change-password
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "mevcutSifre123",
  "newPassword": "yeniSifre456",
  "confirmPassword": "yeniSifre456"
}
```

## 📱 Frontend Kullanım Örnekleri

### JavaScript/TypeScript Örnekleri

#### Etkinlik Ekleme
```javascript
const createEvent = async (eventData) => {
  try {
    const response = await fetch('/api/events', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(eventData)
    });
    
    if (response.ok) {
      const event = await response.json();
      console.log('Etkinlik oluşturuldu:', event);
      return event;
    } else {
      throw new Error('Etkinlik oluşturulamadı');
    }
  } catch (error) {
    console.error('Hata:', error);
    throw error;
  }
};
```

#### Kullanıcı Ayarlarını Güncelleme
```javascript
const updateUserSettings = async (settingsData) => {
  try {
    const response = await fetch('/api/usersettings', {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(settingsData)
    });
    
    if (response.ok) {
      const settings = await response.json();
      console.log('Ayarlar güncellendi:', settings);
      return settings;
    } else {
      throw new Error('Ayarlar güncellenemedi');
    }
  } catch (error) {
    console.error('Hata:', error);
    throw error;
  }
};
```

#### Şifre Değiştirme
```javascript
const changePassword = async (passwordData) => {
  try {
    const response = await fetch('/api/users/change-password', {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(passwordData)
    });
    
    if (response.ok) {
      const result = await response.json();
      console.log('Şifre değiştirildi:', result.message);
      return result;
    } else {
      const error = await response.json();
      throw new Error(error.message);
    }
  } catch (error) {
    console.error('Hata:', error);
    throw error;
  }
};
```

### React Hook Örnekleri

#### Etkinlik Yönetimi Hook'u
```javascript
import { useState, useEffect } from 'react';

export const useEvents = () => {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchEvents = async () => {
    setLoading(true);
    try {
      const response = await fetch('/api/events', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        const data = await response.json();
        setEvents(data);
      } else {
        throw new Error('Etkinlikler yüklenemedi');
      }
    } catch (error) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };

  const addEvent = async (eventData) => {
    try {
      const response = await fetch('/api/events', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(eventData)
      });
      
      if (response.ok) {
        const newEvent = await response.json();
        setEvents(prev => [...prev, newEvent]);
        return newEvent;
      } else {
        throw new Error('Etkinlik eklenemedi');
      }
    } catch (error) {
      setError(error.message);
      throw error;
    }
  };

  useEffect(() => {
    fetchEvents();
  }, []);

  return { events, loading, error, fetchEvents, addEvent };
};
```

## 🚀 Önemli Notlar

1. **JWT Token**: Tüm isteklerde geçerli JWT token kullanılmalıdır
2. **Error Handling**: API hatalarını yakalamak için try-catch blokları kullanın
3. **Loading States**: Kullanıcı deneyimi için loading durumlarını yönetin
4. **Validation**: Frontend'de form validation yapın, backend'de de ek validation vardır
5. **Responsive Design**: Tüm ekran boyutlarında çalışacak şekilde tasarlayın

## 🔧 Hata Kodları

- `400 Bad Request`: Geçersiz veri formatı
- `401 Unauthorized`: Geçersiz veya eksik token
- `403 Forbidden`: Yetkisiz erişim
- `404 Not Found`: Kaynak bulunamadı
- `500 Internal Server Error`: Sunucu hatası

## 🤖 AI Özellikleri API

### Endpoint'ler

#### 1. Survey Analizi
```http
POST /api/ai/analyze-survey/{surveyId}?analysisType=general
Authorization: Bearer {token}
```

#### 2. Duygu Analizi
```http
POST /api/ai/sentiment-analysis
Authorization: Bearer {token}
Content-Type: application/json

[
  "Bu anket çok yararlıydı",
  "Sorular biraz karışıktı", 
  "Genel olarak memnunum"
]
```

#### 3. Anket İçgörüleri
```http
GET /api/ai/insights/{surveyId}
Authorization: Bearer {token}
```

#### 4. Anket Özeti Üret
```http
GET /api/ai/summary/{surveyId}
Authorization: Bearer {token}
```

#### 5. Akıllı Soru Üretimi
```http
POST /api/ai/generate-questions
Authorization: Bearer {token}
Content-Type: application/json

{
  "surveyTitle": "Müşteri Memnuniyeti Anketi",
  "surveyDescription": "Ürünlerimiz hakkında müşteri geri bildirimlerini topluyoruz",
  "category": "customer_satisfaction",
  "existingQuestions": [
    "Ürünümüzden ne kadar memnunsunuz?"
  ],
  "numberOfSuggestions": 5
}
```

#### 6. Cevap Seçenekleri Öner
```http
POST /api/ai/suggest-choices
Authorization: Bearer {token}
Content-Type: application/json

{
  "questionText": "Ürünümüzden ne kadar memnunsunuz?",
  "questionType": "multiple_choice"
}
```

#### 7. Soru Metni İyileştir
```http
POST /api/ai/improve-question
Authorization: Bearer {token}
Content-Type: application/json

{
  "questionText": "Ürünümüz iyi mi?"
}
```

#### 8. Anahtar Kelime Çıkarımı
```http
POST /api/ai/extract-keywords
Authorization: Bearer {token}
Content-Type: application/json

{
  "text": "Bu ürün gerçekten harika. Kalitesi çok yüksek ve fiyatı uygun."
}
```

#### 9. Metin Çeviri
```http
POST /api/ai/translate
Authorization: Bearer {token}
Content-Type: application/json

{
  "text": "Bu anket çok yararlıydı",
  "targetLanguage": "English"
}
```

#### 10. AI Rapor Üretimi
```http
GET /api/ai/report/{surveyId}?reportType=comprehensive
Authorization: Bearer {token}
```

#### 11. AI ile Tam Anket Oluşturma ⭐
```http
POST /api/ai/generate-complete-survey
Authorization: Bearer {token}
Content-Type: application/json

{
  "description": "müşteri memnuniyeti anketi hazırla"
}
```

**Response**: Tam oluşturulmuş Survey objesi (sorularla birlikte)

### AI Kullanım Örnekleri

#### Survey Analizi
```javascript
const analyzeSurvey = async (surveyId, analysisType = 'general') => {
  try {
    const response = await fetch(`/api/ai/analyze-survey/${surveyId}?analysisType=${analysisType}`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
    
    if (response.ok) {
      const analysis = await response.json();
      console.log('AI Analizi:', analysis);
      return analysis;
    }
  } catch (error) {
    console.error('Analiz hatası:', error);
  }
};
```

#### Akıllı Soru Üretimi
```javascript
const generateSmartQuestions = async (surveyData) => {
  try {
    const response = await fetch('/api/ai/generate-questions', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(surveyData)
    });
    
    if (response.ok) {
      const questions = await response.json();
      console.log('AI Önerilen Sorular:', questions);
      return questions;
    }
  } catch (error) {
    console.error('Soru üretimi hatası:', error);
  }
};
```

#### Duygu Analizi
```javascript
const analyzeSentiment = async (responses) => {
  try {
    const response = await fetch('/api/ai/sentiment-analysis', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(responses)
    });
    
    if (response.ok) {
      const sentiment = await response.json();
      console.log('Duygu Analizi:', sentiment);
      return sentiment;
    }
  } catch (error) {
    console.error('Duygu analizi hatası:', error);
  }
};
```

#### AI ile Tam Anket Oluşturma ⭐
```javascript
const generateCompleteSurvey = async (description) => {
  try {
    const response = await fetch('/api/ai/generate-complete-survey', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({ description })
    });
    
    if (response.ok) {
      const survey = await response.json();
      console.log('AI Oluşturulan Anket:', survey);
      
      // Anket oluşturuldu, kullanıcıyı düzenleme sayfasına yönlendir
      window.location.href = `/surveys/edit/${survey.id}`;
      
      return survey;
    } else {
      const error = await response.json();
      throw new Error(error.message || 'Anket oluşturulamadı');
    }
  } catch (error) {
    console.error('AI anket oluşturma hatası:', error);
    throw error;
  }
};

// Kullanım örneği:
document.getElementById('ai-survey-button').addEventListener('click', async () => {
  const description = document.getElementById('survey-description').value;
  
  if (!description.trim()) {
    alert('Lütfen anket açıklaması girin');
    return;
  }
  
  try {
    // Loading durumunu göster
    showLoading(true);
    
    await generateCompleteSurvey(description);
    
    // Success mesajı
    showSuccessMessage('Anket başarıyla oluşturuldu!');
    
  } catch (error) {
    showErrorMessage('Anket oluşturulurken hata oluştu: ' + error.message);
  } finally {
    showLoading(false);
  }
});
```

## 🎯 AI Özellikleri

### 1. **Survey Analizi**
- Anket sonuçlarının AI ile analizi
- Farklı analiz türleri: genel, detaylı, trend analizi
- Güven skoru ile sonuçlar

### 2. **Duygu Analizi**
- Anket cevaplarının duygusal tonunu analiz etme
- Pozitif, negatif, nötr yüzde dağılımları
- Ana pozitif ve negatif temalar

### 3. **Akıllı Soru Önerileri**
- AI destekli soru üretimi
- Mevcut sorulara benzer olmayan öneriler
- Farklı soru tiplerinde destek

### 4. **Anket İçgörüleri**
- Anket verilerinden çıkarılan önemli bulgular
- Kategori bazında gruplandırma
- Önem seviyelerine göre sıralama

### 5. **Otomatik Rapor Üretimi**
- Kapsamlı anket raporları
- Grafik önerileri
- Sonuç ve öneriler

## 📞 Destek

Herhangi bir sorun yaşarsanız, backend ekibi ile iletişime geçin.
