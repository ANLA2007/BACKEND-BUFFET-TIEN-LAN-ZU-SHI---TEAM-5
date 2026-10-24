using Microsoft.AspNetCore.Mvc;

namespace TIEN_LAN_ZU_SHI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class TestController : ControllerBase
    {
        [HttpGet("ok")]
        public IActionResult Ok_() => Ok(new { mensaje = "Todo bien" });

        [HttpGet("fail")]
        public IActionResult Fail()
        {
            throw new InvalidOperationException("Falla intencional para probar el log de errores");
        }
    }
}