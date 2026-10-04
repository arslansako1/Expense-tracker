

using Microsoft.AspNetCore.Mvc;

public class HealthController: ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Ok",
            timestamp = "DateTime.UtcNow",
            version = "1.0.0"
        });
    }
}