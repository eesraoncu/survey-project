using SurveyApp.Models;
using MongoDB.Driver;

namespace SurveyApp.Services;

public interface IAddressService
{
    Task<Address?> CreateAddressAsync(string? cityName, string? districtName, string? districtTownshipTownName, string? neighbourhoodName, string? addressDetails);
    Task<Address> GetAddressAsync(int addressId);
    Task<bool> ValidateAddressAsync(string cityName, string districtName, string districtTownshipTownName, string neighbourhoodName);
    
    // Dropdown'lar için gerekli metodlar
    Task<List<City>> GetAllCitiesAsync();
    Task<List<District>> GetDistrictsByCityAsync(string cityName);
    Task<List<DistrictTownshipTown>> GetDistrictTownshipTownsByDistrictAsync(string cityName, string districtName);
    Task<List<Neighbourhood>> GetNeighbourhoodsByDistrictTownshipTownAsync(string cityName, string districtName, string districtTownshipTownName);
}

public class AddressService : IAddressService
{
    private readonly IMongoDatabase _database;

    public AddressService(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<Address?> CreateAddressAsync(string? cityName, string? districtName, string? districtTownshipTownName, string? neighbourhoodName, string? addressDetails)
    {
        // Eğer adres bilgileri boşsa null döndür
        if (string.IsNullOrWhiteSpace(cityName) || 
            string.IsNullOrWhiteSpace(districtName) || 
            string.IsNullOrWhiteSpace(districtTownshipTownName) || 
            string.IsNullOrWhiteSpace(neighbourhoodName))
        {
            return null;
        }

        // Adres geçerliliğini kontrol et
        if (!await ValidateAddressAsync(cityName, districtName, districtTownshipTownName, neighbourhoodName))
        {
            throw new ArgumentException("Geçersiz adres bilgileri!");
        }

        // Yeni adres oluştur
        var address = new Address
        {
            NeighbourhoodId = await GetNeighbourhoodIdByNameAsync(cityName, districtName, districtTownshipTownName, neighbourhoodName),
            AddressDetails = addressDetails ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        // AutoIncrementService ile ID al
        var addressCollection = _database.GetCollection<Address>("address");
        var maxId = await addressCollection.Find(_ => true).SortByDescending(x => x.Id).Limit(1).FirstOrDefaultAsync();
        address.Id = (maxId?.Id ?? 0) + 1;

        await addressCollection.InsertOneAsync(address);
        return address;
    }

    public async Task<Address> GetAddressAsync(int addressId)
    {
        var addressCollection = _database.GetCollection<Address>("address");
        return await addressCollection.Find(x => x.Id == addressId).FirstOrDefaultAsync();
    }

    public async Task<bool> ValidateAddressAsync(string cityName, string districtName, string districtTownshipTownName, string neighbourhoodName)
    {
        try
        {
            var city = await GetCityByNameAsync(cityName);
            if (city == null) return false;

            var district = await GetDistrictByNameAsync(cityName, districtName);
            if (district == null) return false;

            var dtt = await GetDistrictTownshipTownByNameAsync(cityName, districtName, districtTownshipTownName);
            if (dtt == null) return false;

            var neighbourhood = await GetNeighbourhoodByNameAsync(cityName, districtName, districtTownshipTownName, neighbourhoodName);
            if (neighbourhood == null) return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    // Dropdown'lar için gerekli metodlar
    public async Task<List<City>> GetAllCitiesAsync()
    {
        var cityCollection = _database.GetCollection<City>("city");
        return await cityCollection.Find(_ => true).ToListAsync();
    }

    public async Task<List<District>> GetDistrictsByCityAsync(string cityName)
    {
        var city = await GetCityByNameAsync(cityName);
        if (city == null) return new List<District>();

        var districtCollection = _database.GetCollection<District>("district");
        return await districtCollection.Find(x => x.CityId == city.Id).ToListAsync();
    }

    public async Task<List<DistrictTownshipTown>> GetDistrictTownshipTownsByDistrictAsync(string cityName, string districtName)
    {
        var district = await GetDistrictByNameAsync(cityName, districtName);
        if (district == null) return new List<DistrictTownshipTown>();

        var dttCollection = _database.GetCollection<DistrictTownshipTown>("district_township_town");
        return await dttCollection.Find(x => x.DistrictId == district.Id).ToListAsync();
    }

    public async Task<List<Neighbourhood>> GetNeighbourhoodsByDistrictTownshipTownAsync(string cityName, string districtName, string districtTownshipTownName)
    {
        var dtt = await GetDistrictTownshipTownByNameAsync(cityName, districtName, districtTownshipTownName);
        if (dtt == null) return new List<Neighbourhood>();

        var neighbourhoodCollection = _database.GetCollection<Neighbourhood>("neighbourhood");
        return await neighbourhoodCollection.Find(x => x.DistrictTownshipTownId == dtt.Id).ToListAsync();
    }

    // Private helper metodlar
    private async Task<City?> GetCityByNameAsync(string cityName)
    {
        var cityCollection = _database.GetCollection<City>("city");
        return await cityCollection.Find(x => x.CityName == cityName).FirstOrDefaultAsync();
    }

    private async Task<District?> GetDistrictByNameAsync(string cityName, string districtName)
    {
        var city = await GetCityByNameAsync(cityName);
        if (city == null) return null;

        var districtCollection = _database.GetCollection<District>("district");
        return await districtCollection.Find(x => x.DistrictName == districtName && x.CityId == city.Id).FirstOrDefaultAsync();
    }

    private async Task<DistrictTownshipTown?> GetDistrictTownshipTownByNameAsync(string cityName, string districtName, string districtTownshipTownName)
    {
        var district = await GetDistrictByNameAsync(cityName, districtName);
        if (district == null) return null;

        var dttCollection = _database.GetCollection<DistrictTownshipTown>("district_township_town");
        return await dttCollection.Find(x => x.DistrictTownshipTownName == districtTownshipTownName && x.DistrictId == district.Id).FirstOrDefaultAsync();
    }

    private async Task<Neighbourhood?> GetNeighbourhoodByNameAsync(string cityName, string districtName, string districtTownshipTownName, string neighbourhoodName)
    {
        var dtt = await GetDistrictTownshipTownByNameAsync(cityName, districtName, districtTownshipTownName);
        if (dtt == null) return null;

        var neighbourhoodCollection = _database.GetCollection<Neighbourhood>("neighbourhood");
        return await neighbourhoodCollection.Find(x => x.NeighbourhoodName == neighbourhoodName && x.DistrictTownshipTownId == dtt.Id).FirstOrDefaultAsync();
    }

    private async Task<int> GetNeighbourhoodIdByNameAsync(string cityName, string districtName, string districtTownshipTownName, string neighbourhoodName)
    {
        var neighbourhood = await GetNeighbourhoodByNameAsync(cityName, districtName, districtTownshipTownName, neighbourhoodName);
        return neighbourhood?.Id ?? 0;
    }
}
