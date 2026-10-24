using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;


[ApiController]
[Route("Map")]
public class MapController : ControllerBase
{
    private readonly ServiceMap _service;

    public MapController(ServiceMap service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMap()
    {
        var maps = await _service.GetAllMap();
        return Ok(maps);
    }

    [HttpGet("uuid/{UUID}")]
    public async Task<IActionResult> GetAllMapbyUUID(string UUID)
    {
        var maps = await _service.GetAllMapbyUUID(UUID);
        return Ok(maps);
    }

    [HttpGet("{ID}")]
    public async Task<IActionResult> GetMapbyID(int ID)
    {
        var maps = await _service.GetAllMapbyID(ID);
        return Ok(maps);
    }

    [HttpGet("device")]
    public async Task<IActionResult> GetDevice()
    {
        var maps = await _service.GetAllDevice();
        return Ok(maps);
    }

    [HttpGet("nickname/{Nickname}")]
    public async Task<IActionResult> GetNickname(string Nickname)
    {
        var maps = await _service.GetNickname(Nickname);
        return Ok(maps);
    }

    [HttpPost]
    public async Task<IActionResult> CreateIncidentReport([FromBody] JObject data)
    {   
        var report = await _service.AddIncidentReport(data);
        return Ok(report);
    }
    
    [HttpPost("image")]
    public async Task<IActionResult> AddIncidentImage([FromBody] JObject data)
    {
        var report = await _service.AddIncidentImage(data);
        return Ok(report);
    }

    [HttpPost("nickname")]
    public async Task<IActionResult> AddUserName([FromBody] JObject data)
    {
        var report = await _service.AddUserName(data);
        return Ok(report);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncidentReport(int id, [FromBody] JObject data)
    {
        var report = await _service.UpdateIncidentReport(id, data);
        return Ok(report);
    }

    [HttpPut("solve/{id}")]
    public async Task<IActionResult> SolveIncidentReport(int id)
    {
        var report = await _service.SolveIncidentReport(id);
        return Ok(report);
    }

    [HttpPut("unsolve/{id}")]
    public async Task<IActionResult> unSolveIncidentReport(int id)
    {
        var report = await _service.unSolveIncidentReport(id);
        return Ok(report);
    }

    [HttpPost("add-chart")]
    public async Task<IActionResult> AddChart([FromBody] JObject data)
    {
        try
        {
            var chart = await _service.AddChart(data);
            return Ok(chart);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("fake/{id}")]
    public async Task<IActionResult> FakeIncidentReport(int id)
    {
        var report = await _service.FakeIncidentReport(id);
        return Ok(report);
    }

    [HttpPut("unfake/{id}")]
    public async Task<IActionResult> unFakeIncidentReport(int id)
    {
        var report = await _service.unFakeIncidentReport(id);
        return Ok(report);
    }
}
