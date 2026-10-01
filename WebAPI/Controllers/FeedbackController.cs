using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FeedbackController : Controller
    {
        private readonly IDataService _dataService;
        public FeedbackController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.feedback.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var categories = await _dataService.feedback.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Home Page Banners Menu not found");
            return Ok(categories);
        }

        [HttpPost]
        [AllowAnonymous]
        public Task<IActionResult> Creates()
        {
            //if (argModelDto == null) return BadRequest("Input not valid or null");
            //if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            //var modelDto = await _dataService.feedback.Create(argModelDto).ConfigureAwait(false);
            //if (modelDto == null) return BadRequest("Create failed");
            //return Ok(modelDto);
            return null;
        }

    }
}
