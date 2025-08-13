using Microsoft.AspNetCore.Mvc;
using SurveyApp.Services;
using SurveyApp.Models;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet("cities")]
    public async Task<ActionResult<List<City>>> GetAllCities()
    {
        try
        {
            var cities = await _addressService.GetAllCitiesAsync();
            return Ok(cities);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Şehirler getirilemedi!", error = ex.Message });
        }
    }

    [HttpGet("districts/{cityName}")]
    public async Task<ActionResult<List<District>>> GetDistrictsByCity(string cityName)
    {
        try
        {
            var districts = await _addressService.GetDistrictsByCityAsync(cityName);
            return Ok(districts);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "İlçeler getirilemedi!", error = ex.Message });
        }
    }

    [HttpGet("district-township-towns/{cityName}/{districtName}")]
    public async Task<ActionResult<List<DistrictTownshipTown>>> GetDistrictTownshipTownsByDistrict(string cityName, string districtName)
    {
        try
        {
            var districtTownshipTowns = await _addressService.GetDistrictTownshipTownsByDistrictAsync(cityName, districtName);
            return Ok(districtTownshipTowns);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Semtler getirilemedi!", error = ex.Message });
        }
    }

    [HttpGet("neighbourhoods/{cityName}/{districtName}/{districtTownshipTownName}")]
    public async Task<ActionResult<List<Neighbourhood>>> GetNeighbourhoodsByDistrictTownshipTown(string cityName, string districtName, string districtTownshipTownName)
    {
        try
        {
            var neighbourhoods = await _addressService.GetNeighbourhoodsByDistrictTownshipTownAsync(cityName, districtName, districtTownshipTownName);
            return Ok(neighbourhoods);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Mahalleler getirilemedi!", error = ex.Message });
        }
    }
}
