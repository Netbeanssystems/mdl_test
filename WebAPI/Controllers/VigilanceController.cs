using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class VigilanceController : Controller
    {
        private readonly IDataService _dataService;
        public VigilanceController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VigilanceFormDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.vigilance.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
    }
}
