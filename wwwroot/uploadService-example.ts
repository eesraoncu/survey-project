// uploadService.ts - Frontend için doğru implementasyon
import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

export const uploadService = {
  // Resim yükleme fonksiyonu
  async uploadImage(file: File): Promise<string> {
    try {
      console.log('📤 Resim yükleme isteği gönderiliyor...');
      console.log('📁 Dosya adı:', file.name);
      console.log('📏 Dosya boyutu:', file.size, 'bytes');
      console.log('🔗 API URL:', `${API_BASE_URL}/upload/image`);

      const formData = new FormData();
      formData.append('file', file);

      const response = await axios.post(`${API_BASE_URL}/upload/image`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
        timeout: 30000, // 30 saniye timeout
      });

      console.log('✅ Resim yükleme başarılı:', response.data);
      return response.data.fileUrl; // Backend'den dönen fileUrl'i döndür

    } catch (error: any) {
      console.error('❌ Resim yükleme hatası:', error);
      console.log('📋 Hata detayları:', {
        status: error.response?.status,
        statusText: error.response?.statusText,
        data: error.response?.data,
        message: error.message,
        config: error.config
      });

      if (error.response?.data) {
        console.log('🔍 Backend hata mesajı:', error.response.data);
        console.log('📝 Backend mesaj detayı:', error.response.data.message);
      }

      // Hata türüne göre mesaj döndür
      if (error.response?.status === 400) {
        if (error.response.data?.message) {
          throw new Error(error.response.data.message);
        } else {
          throw new Error('Geçersiz dosya formatı');
        }
      } else if (error.response?.status === 413) {
        throw new Error('Dosya boyutu çok büyük');
      } else if (error.code === 'ECONNABORTED') {
        throw new Error('Yükleme zaman aşımına uğradı');
      } else {
        throw new Error('Resim yüklenirken bir hata oluştu');
      }
    }
  },

  // Arka plan resmi yükleme fonksiyonu
  async uploadBackgroundImage(file: File): Promise<string> {
    try {
      console.log('🎨 Arka plan resmi yükleniyor...');
      const fileUrl = await this.uploadImage(file);
      console.log('✅ Arka plan resmi yükleme başarılı:', fileUrl);
      return fileUrl;
    } catch (error: any) {
      console.error('❌ Arka plan resmi yükleme hatası:', error);
      throw error;
    }
  }
};

export default uploadService;
