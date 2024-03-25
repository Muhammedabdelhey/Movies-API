using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Movies_Core_Layer.Models;

namespace Movies_With_Reopsitory_Pattren.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        //singletoone
        //private readonly IOptions<AttachmentsOptions> _attachmentsOptions;
        //scoped
        //private readonly IOptionsSnapshot <AttachmentsOptions> _attachmentsOptions;
        //Transient
        private readonly IOptionsMonitor<AttachmentsOptions> _attachmentsOptions;

        public ConfigController(IConfiguration configuration, IOptionsMonitor<AttachmentsOptions> attachmentsOptions)
        {
            _configuration = configuration;
            _attachmentsOptions = attachmentsOptions;
        }
        [HttpGet]
        public IActionResult GetConfig()
        {
            return Ok(new
            {
                AllowedHosts = _configuration["AllowedHosts"],
                Logging = _configuration["Logging"],
                ConnectionStrings = _configuration["ConnectionStrings:DefaultConnection"],
                AttachmentsOptions = _attachmentsOptions.CurrentValue
            });
        }
    }
}
