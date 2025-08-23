// FormBuilder.tsx - Arka plan resmi kullanım örneği
import React, { useState } from 'react';
import { uploadService } from './uploadService-example';

interface Survey {
  id: string;
  surveyName: string;
  surveyDescription: string;
  usersId: number;
  surveyTypeId: number;
  createdAt: string;
  isActive: boolean;
  surveyBackgroundImage?: string; // Arka plan resmi URL'i
}

interface SurveyFormData {
  surveyName: string;
  surveyDescription: string;
  surveyTypeId: number;
  isActive: boolean;
  surveyBackgroundImage?: string; // Arka plan resmi URL'i
}

const FormBuilder: React.FC = () => {
  const [formData, setFormData] = useState<SurveyFormData>({
    surveyName: '',
    surveyDescription: '',
    surveyTypeId: 1,
    isActive: true,
    surveyBackgroundImage: undefined
  });

  const [isUploading, setIsUploading] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);

  // Resim yükleme işlemi
  const handleImageUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Dosya türü kontrolü
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      setUploadError('Sadece resim dosyaları kabul edilir (JPG, PNG, GIF, WEBP)');
      return;
    }

    // Dosya boyutu kontrolü (5MB)
    if (file.size > 5 * 1024 * 1024) {
      setUploadError('Dosya boyutu 5MB\'dan küçük olmalıdır');
      return;
    }

    setIsUploading(true);
    setUploadError(null);

    try {
      console.log('🎨 FormBuilder - Resim yükleme başlatılıyor...');
      const fileUrl = await uploadService.uploadBackgroundImage(file);
      
      setFormData(prev => ({
        ...prev,
        surveyBackgroundImage: fileUrl
      }));
      
      console.log('✅ FormBuilder - Resim yükleme başarılı:', fileUrl);
      
    } catch (error: any) {
      console.error('❌ FormBuilder - Resim yükleme hatası:', error);
      console.log('📋 Hata detayları:', {
        error: error,
        message: error.message,
        stack: error.stack
      });
      
      setUploadError(error.message || 'Resim yüklenirken bir hata oluştu');
    } finally {
      setIsUploading(false);
    }
  };

  // Anket kaydetme işlemi
  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    
    try {
      console.log('📝 Anket kaydediliyor:', formData);
      
      // API çağrısı burada yapılacak
      const response = await fetch('http://localhost:5000/api/surveys', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData)
      });

      if (response.ok) {
        const result = await response.json();
        console.log('✅ Anket başarıyla kaydedildi:', result);
        alert('Anket başarıyla kaydedildi!');
      } else {
        throw new Error('Anket kaydedilemedi');
      }
      
    } catch (error: any) {
      console.error('❌ Anket kaydetme hatası:', error);
      alert('Anket kaydedilirken bir hata oluştu');
    }
  };

  return (
    <div className="form-builder">
      <h2>Anket Oluşturucu</h2>
      
      <form onSubmit={handleSubmit}>
        {/* Anket Adı */}
        <div className="form-group">
          <label htmlFor="surveyName">Anket Adı:</label>
          <input
            type="text"
            id="surveyName"
            value={formData.surveyName}
            onChange={(e) => setFormData(prev => ({ ...prev, surveyName: e.target.value }))}
            required
          />
        </div>

        {/* Anket Açıklaması */}
        <div className="form-group">
          <label htmlFor="surveyDescription">Anket Açıklaması:</label>
          <textarea
            id="surveyDescription"
            value={formData.surveyDescription}
            onChange={(e) => setFormData(prev => ({ ...prev, surveyDescription: e.target.value }))}
            required
          />
        </div>

        {/* Arka Plan Resmi */}
        <div className="form-group">
          <label htmlFor="backgroundImage">Arka Plan Resmi:</label>
          <input
            type="file"
            id="backgroundImage"
            accept="image/*"
            onChange={handleImageUpload}
            disabled={isUploading}
          />
          
          {isUploading && <p>🔄 Resim yükleniyor...</p>}
          
          {uploadError && (
            <p className="error">❌ {uploadError}</p>
          )}
          
          {formData.surveyBackgroundImage && (
            <div className="preview">
              <p>✅ Resim yüklendi:</p>
              <img 
                src={`http://localhost:5000${formData.surveyBackgroundImage}`} 
                alt="Arka plan önizleme"
                style={{ maxWidth: '200px', maxHeight: '200px' }}
              />
              <p>URL: {formData.surveyBackgroundImage}</p>
            </div>
          )}
        </div>

        {/* Anket Türü */}
        <div className="form-group">
          <label htmlFor="surveyTypeId">Anket Türü:</label>
          <select
            id="surveyTypeId"
            value={formData.surveyTypeId}
            onChange={(e) => setFormData(prev => ({ ...prev, surveyTypeId: parseInt(e.target.value) }))}
          >
            <option value={1}>Genel Anket</option>
            <option value={2}>Müşteri Memnuniyeti</option>
            <option value={3}>Çalışan Anketi</option>
          </select>
        </div>

        {/* Aktif/Pasif */}
        <div className="form-group">
          <label>
            <input
              type="checkbox"
              checked={formData.isActive}
              onChange={(e) => setFormData(prev => ({ ...prev, isActive: e.target.checked }))}
            />
            Anket Aktif
          </label>
        </div>

        <button type="submit" disabled={isUploading}>
          {isUploading ? 'Kaydediliyor...' : 'Anketi Kaydet'}
        </button>
      </form>
    </div>
  );
};

export default FormBuilder;
