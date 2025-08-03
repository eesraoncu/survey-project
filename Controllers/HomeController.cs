using Microsoft.AspNetCore.Mvc;

namespace SurveyApp.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("ASP.NET Core API çalışıyor!");
    }
} 