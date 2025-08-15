using Microsoft.AspNetCore.Mvc;
using SurveyApp.Services;
using SurveyApp.Models;

namespace SurveyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AdresController : ControllerBase
{
    private readonly IAdresService _adresService;

    public AdresController(IAdresService adresService)
    {
        _adresService = adresService;
    }

    [HttpGet("iller")]
    public async Task<ActionResult<List<Il>>> GetAllIller()
    {
        try
        {
            var iller = await _adresService.GetAllIllerAsync();
            return Ok(iller);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "İller getirilemedi!", error = ex.Message });
        }
    }

    [HttpGet("ilceler/{il}")]
    public async Task<ActionResult<List<Ilce>>> GetIlcelerByIl(string il)
    {
        try
        {
            var ilceler = await _adresService.GetIlcelerByIlAsync(il);
            return Ok(ilceler);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "İlçeler getirilemedi!", error = ex.Message });
        }
    }

    [HttpGet("semtler/{il}/{ilce}")]
    public async Task<ActionResult<List<SemtBucakBelde>>> GetSemtlerByIlce(string il, string ilce)
    {
        try
        {
            var semtler = await _adresService.GetSemtlerByIlceAsync(il, ilce);
            return Ok(semtler);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Semtler getirilemedi!", error = ex.Message });
        }
    }

    [HttpGet("mahalleler/{il}/{ilce}/{semt_bucak_belde}")]
    public async Task<ActionResult<List<Mahalle>>> GetMahallelerBySemt(string il, string ilce, string semt_bucak_belde)
    {
        try
        {
            var mahalleler = await _adresService.GetMahallelerBySemtAsync(il, ilce, semt_bucak_belde);
            return Ok(mahalleler);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Mahalleler getirilemedi!", error = ex.Message });
        }
    }
}
