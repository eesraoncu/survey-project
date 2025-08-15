using SurveyApp.Models;
using MongoDB.Driver;

namespace SurveyApp.Services;

public interface IAdresService
{
    Task<Adres?> CreateAdresAsync(string? il, string? ilce, string? semt_bucak_belde, string? mahalle, string? adresDetay);
    Task<Adres> GetAdresAsync(int adresId);
    Task<bool> ValidateAdresAsync(string il, string ilce, string semt_bucak_belde, string mahalle);
    
    // Dropdown'lar için gerekli metodlar
    Task<List<Il>> GetAllIllerAsync();
    Task<List<Ilce>> GetIlcelerByIlAsync(string il);
    Task<List<SemtBucakBelde>> GetSemtlerByIlceAsync(string il, string ilce);
    Task<List<Mahalle>> GetMahallelerBySemtAsync(string il, string ilce, string semt_bucak_belde);
}

public class AdresService : IAdresService
{
    private readonly IMongoDatabase _database;

    public AdresService(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<Adres?> CreateAdresAsync(string? il, string? ilce, string? semt_bucak_belde, string? mahalle, string? adresDetay)
    {
        // Eğer adres bilgileri boşsa null döndür
        if (string.IsNullOrWhiteSpace(il) || 
            string.IsNullOrWhiteSpace(ilce) || 
            string.IsNullOrWhiteSpace(semt_bucak_belde) || 
            string.IsNullOrWhiteSpace(mahalle))
        {
            return null;
        }

        // Adres geçerliliğini kontrol et
        if (!await ValidateAdresAsync(il, ilce, semt_bucak_belde, mahalle))
        {
            throw new ArgumentException("Geçersiz adres bilgileri!");
        }

        // Yeni adres oluştur
        var adres = new Adres
        {
            MahalleId = await GetMahalleIdByNameAsync(il, ilce, semt_bucak_belde, mahalle),
            AdresDetay = adresDetay ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        // AutoIncrementService ile ID al
        var adresCollection = _database.GetCollection<Adres>("address");
        var maxId = await adresCollection.Find(_ => true).SortByDescending(x => x.Id).Limit(1).FirstOrDefaultAsync();
        adres.Id = (maxId?.Id ?? 0) + 1;

        await adresCollection.InsertOneAsync(adres);
        return adres;
    }

    public async Task<Adres> GetAdresAsync(int adresId)
    {
        var adresCollection = _database.GetCollection<Adres>("address");
        return await adresCollection.Find(x => x.Id == adresId).FirstOrDefaultAsync();
    }

    public async Task<bool> ValidateAdresAsync(string il, string ilce, string semt_bucak_belde, string mahalle)
    {
        try
        {
            var ilData = await GetIlByNameAsync(il);
            if (ilData == null) return false;

            var ilceData = await GetIlceByNameAsync(il, ilce);
            if (ilceData == null) return false;

            var semtData = await GetSemtByNameAsync(il, ilce, semt_bucak_belde);
            if (semtData == null) return false;

            var mahalleData = await GetMahalleByNameAsync(il, ilce, semt_bucak_belde, mahalle);
            if (mahalleData == null) return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    // Dropdown'lar için gerekli metodlar
    public async Task<List<Il>> GetAllIllerAsync()
    {
        var ilCollection = _database.GetCollection<Il>("il");
        return await ilCollection.Find(_ => true).ToListAsync();
    }

    public async Task<List<Ilce>> GetIlcelerByIlAsync(string il)
    {
        var ilData = await GetIlByNameAsync(il);
        if (ilData == null) return new List<Ilce>();

        var ilceCollection = _database.GetCollection<Ilce>("ilce");
        return await ilceCollection.Find(x => x.IlId == ilData.IlId).ToListAsync();
    }

    public async Task<List<SemtBucakBelde>> GetSemtlerByIlceAsync(string il, string ilce)
    {
        var ilceData = await GetIlceByNameAsync(il, ilce);
        if (ilceData == null) return new List<SemtBucakBelde>();

        var semtCollection = _database.GetCollection<SemtBucakBelde>("semt_bucak_belde");
        return await semtCollection.Find(x => x.IlceId == ilceData.IlceId).ToListAsync();
    }

    public async Task<List<Mahalle>> GetMahallelerBySemtAsync(string il, string ilce, string semt_bucak_belde)
    {
        var semtData = await GetSemtByNameAsync(il, ilce, semt_bucak_belde);
        if (semtData == null) return new List<Mahalle>();

        var mahalleCollection = _database.GetCollection<Mahalle>("mahalle");
        return await mahalleCollection.Find(x => x.SemtBucakBeldeId == semtData.Id).ToListAsync();
    }

    // Private helper metodlar
    private async Task<Il?> GetIlByNameAsync(string il)
    {
        var ilCollection = _database.GetCollection<Il>("il");
        return await ilCollection.Find(x => x.IlAdi == il).FirstOrDefaultAsync();
    }

    private async Task<Ilce?> GetIlceByNameAsync(string il, string ilce)
    {
        var ilData = await GetIlByNameAsync(il);
        if (ilData == null) return null;

        var ilceCollection = _database.GetCollection<Ilce>("ilce");
        return await ilceCollection.Find(x => x.IlceAdi == ilce && x.IlId == ilData.IlId).FirstOrDefaultAsync();
    }

    private async Task<SemtBucakBelde?> GetSemtByNameAsync(string il, string ilce, string semt_bucak_belde)
    {
        var ilceData = await GetIlceByNameAsync(il, ilce);
        if (ilceData == null) return null;

        var semtCollection = _database.GetCollection<SemtBucakBelde>("semt_bucak_belde");
        return await semtCollection.Find(x => x.SemtBucakBeldeAdi == semt_bucak_belde && x.IlceId == ilceData.IlceId).FirstOrDefaultAsync();
    }

    private async Task<Mahalle?> GetMahalleByNameAsync(string il, string ilce, string semt_bucak_belde, string mahalle)
    {
        var semtData = await GetSemtByNameAsync(il, ilce, semt_bucak_belde);
        if (semtData == null) return null;

        var mahalleCollection = _database.GetCollection<Mahalle>("mahalle");
        return await mahalleCollection.Find(x => x.MahalleAdi == mahalle && x.SemtBucakBeldeId == semtData.Id).FirstOrDefaultAsync();
    }

    private async Task<int> GetMahalleIdByNameAsync(string il, string ilce, string semt_bucak_belde, string mahalle)
    {
        var mahalleData = await GetMahalleByNameAsync(il, ilce, semt_bucak_belde, mahalle);
        return mahalleData?.Id ?? 0;
    }
}
