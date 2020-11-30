using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasmUploadProgressSample.Server.Controllers
{
    [Route("api/[controller]")]
    public class TempFileController : ControllerBase
    {
        [HttpPost("[action]")]
        public IActionResult SaveTempFile(IFormFile file)
        {
            return Ok();
        }
    }
}