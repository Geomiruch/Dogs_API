using Microsoft.AspNetCore.Mvc;

namespace Dogs_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Dogs house service. Version 1.0.1");
        }
    }
}
