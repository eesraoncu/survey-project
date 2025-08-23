// MongoDB'de Activity Log koleksiyonu için önerilen index'ler
// MongoDB shell'de çalıştırın

use SurveyApp; // Veritabanı adınızı kullanın

// 1. Kullanıcı ID index'i (kullanıcı logları için)
db.activity_logs.createIndex({ "user_id": 1 });

// 2. Aktivite tipi index'i (tip bazlı sorgular için)
db.activity_logs.createIndex({ "activity_type": 1 });

// 3. Tarih index'i (zaman bazlı sorgular için)
db.activity_logs.createIndex({ "created_at": -1 });

// 4. Compound index (kullanıcı + tarih)
db.activity_logs.createIndex({ "user_id": 1, "created_at": -1 });

// 5. Compound index (aktivite tipi + tarih)
db.activity_logs.createIndex({ "activity_type": 1, "created_at": -1 });

// 6. Kaynak bazlı sorgular için
db.activity_logs.createIndex({ "resource_type": 1, "resource_id": 1 });

// Index'leri kontrol etmek için:
db.activity_logs.getIndexes();

// Koleksiyon istatistiklerini görmek için:
db.activity_logs.stats();
