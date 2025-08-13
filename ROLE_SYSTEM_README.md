# Survey App Rol Sistemi

## Genel Bakış
Bu proje, kullanıcıların rollerine göre farklı yetkilere sahip olduğu bir anket uygulamasıdır.

## Rol Yapısı
- **1: Admin** - Sistem yöneticisi (sadece manuel eklenebilir)
- **2: Owner** - Anket oluşturan kullanıcı
- **3: User** - Normal kullanıcı (anket dolduran)

## Rol Değişiklik Kuralları

### 1. Yeni Kullanıcı Kaydı
- Yeni kullanıcılar otomatik olarak **User (3)** rolü alır
- Admin rolü sadece veritabanından manuel olarak eklenebilir

### 2. Anket Oluşturma
- Kullanıcı anket oluşturduğunda rolü **Owner (2)** olur
- Admin kullanıcıların rolü değişmez

### 3. Anket Doldurma
- Kullanıcı anket doldurduğunda rolü **User (3)** olur
- Admin kullanıcıların rolü değişmez

## Adres Sistemi

### Hiyerarşik Adres Yapısı
```
City → District → DistrictTownshipTown → Neighbourhood → Address
```

### Kullanıcı Kaydında Adres
- **Frontend dropdown'lardan seçim yapar:**
  - İl seçer → İlçeler yüklenir
  - İlçe seçer → Semtler yüklenir  
  - Semt seçer → Mahalleler yüklenir
  - Mahalle seçer → Adres detayları girer
- **Backend'de:**
  - Adres geçerliliği kontrol edilir
  - Otomatik olarak `address_id` oluşturulur
  - Kullanıcıya `address_id` atanır

### Adres Validation
- Tüm adres bileşenleri hiyerarşik olarak kontrol edilir
- Geçersiz adres bilgileri reddedilir
- Adres detayları (sokak, bina, daire) kullanıcı tarafından girilir

## API Endpoints

### Authentication İşlemleri
- `POST /api/Auth/register` - Yeni kullanıcı kaydı (isim bazlı adres bilgileri ile)
- `POST /api/Auth/login` - Kullanıcı girişi
- `POST /api/Auth/logout` - Kullanıcı çıkışı

### Adres Dropdown'ları
- `GET /api/Address/cities` - Tüm şehirleri listele
- `GET /api/Address/districts/{cityName}` - Şehre göre ilçeleri listele
- `GET /api/Address/district-township-towns/{cityName}/{districtName}` - İlçeye göre semtleri listele
- `GET /api/Address/neighbourhoods/{cityName}/{districtName}/{districtTownshipTownName}` - Semte göre mahalleleri listele

### Kullanıcı İşlemleri
- `GET /api/Users` - Tüm kullanıcıları listele
- `GET /api/Users/{id}` - Kullanıcı getir
- `GET /api/Users/by-role/{roleId}` - Role'e göre kullanıcıları listele
- `PUT /api/Users/{id}` - Kullanıcı bilgilerini güncelle
- `PUT /api/Users/{id}/role` - Rol güncelleme (sadece sistem)

### Anket İşlemleri
- `POST /api/Surveys` - Anket oluşturma (kullanıcıyı Owner yapar)
- `POST /api/Surveys/{id}/complete` - Anket tamamlama (kullanıcıyı User yapar)

### Rol İşlemleri
- `GET /api/role` - Rolleri listele
- `PUT /api/role/{id}` - Rol adını güncelle (ID değiştirilemez)

### Diğer İşlemler
- `GET /api/Questions` - Soruları listele
- `GET /api/Answers` - Cevapları listele

## Kaldırılan Controller'lar
- **TestController** - Geliştirme amaçlı test controller'ı kaldırıldı
- **HomeController** - Basit durum kontrolü controller'ı kaldırıldı  
- **MongoDBController** - MongoDB test controller'ı kaldırıldı

## Yeni Eklenen Controller'lar
- **AuthController** - Giriş, kayıt ve çıkış işlemleri için
- **AddressController** - Adres dropdown'ları için

## Yeni Eklenen Servisler
- **AddressService** - Adres oluşturma, validation ve dropdown verileri

## Frontend Kullanım Örneği

### 1. Şehir Seçimi
```javascript
// Tüm şehirleri getir
const cities = await fetch('/api/Address/cities');
```

### 2. İlçe Seçimi
```javascript
// Seçilen şehre göre ilçeleri getir
const districts = await fetch(`/api/Address/districts/${selectedCity}`);
```

### 3. Semt Seçimi
```javascript
// Seçilen ilçeye göre semtleri getir
const semts = await fetch(`/api/Address/district-township-towns/${selectedCity}/${selectedDistrict}`);
```

### 4. Mahalle Seçimi
```javascript
// Seçilen semte göre mahalleleri getir
const mahalleler = await fetch(`/api/Address/neighbourhoods/${selectedCity}/${selectedDistrict}/${selectedSemt}`);
```

### 5. Kullanıcı Kaydı
```json
{
  "userName": "Ahmet",
  "userSurname": "Yılmaz",
  "userEmail": "ahmet@example.com",
  "userPassword": "123456",
  "userAge": 25,
  "cityName": "İstanbul",
  "districtName": "Kadıköy",
  "districtTownshipTownName": "Fenerbahçe",
  "neighbourhoodName": "Atatürk Mahallesi",
  "addressDetails": "Atatürk Caddesi No:123 Daire:5"
}
```

## Veritabanı Kurulumu

### 1. MongoDB'de Role Collection'ını Düzenle
```bash
# MongoDB Shell'de çalıştır
use your_database_name

# Role collection'ını temizle ve yeniden oluştur
db.role.deleteMany({});
db.role.insertMany([
    { "_id": 1, "role_name": "admin", "is_active": true },
    { "_id": 2, "role_name": "owner", "is_active": true },
    { "_id": 3, "role_name": "user", "is_active": true }
]);
```

### 2. Admin Kullanıcısı Ekle
```javascript
// MongoDB'de admin kullanıcısı ekle
db.users.insertOne({
    "_id": 1,
    "user_name": "Admin",
    "user_surname": "User",
    "user_email": "admin@example.com",
    "user_password": "hashed_password",
    "user_address": "",
    "user_age": 30,
    "address_id": 1,
    "role_id": 1, // Admin rolü
    "created_at": new Date(),
    "is_active": true
});
```

## Güvenlik Notları
- Admin rolü sadece veritabanından manuel olarak eklenebilir
- Kullanıcılar kendi rollerini değiştiremez
- Rol değişiklikleri sadece sistem tarafından yapılır
- Şifre hashleme işlemi implement edilmelidir
- Test ve geliştirme controller'ları production'da kaldırıldı
- Authentication işlemleri ayrı controller'da yönetiliyor
- Adres bilgileri validation ile kontrol ediliyor
- Frontend sadece isim seçer, ID'ler backend'de otomatik oluşturulur

## Test Senaryoları
1. Yeni kullanıcı kaydı → User rolü (3)
2. Anket oluşturma → Owner rolü (2)
3. Anket doldurma → User rolü (3)
4. Admin kullanıcı → Rol değişmez (1)
5. Adres validation → Geçersiz adres reddedilir
6. Dropdown verileri → Hiyerarşik olarak yüklenir
