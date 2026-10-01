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
    public class WebsiteCounterController : Controller
    {
        private readonly IDataService _dataService;
        public WebsiteCounterController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [AllowAnonymous]
        // GET Counter/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.counter.Get(id).ConfigureAwait(false);
            modelDto.LastUpdate = await _dataService.MenuHeadings.GetLastDateTime().ConfigureAwait(false);

            if (modelDto == null) return NotFound("Count not found");
            return Ok(modelDto);
        }

        [AllowAnonymous]
        // PUT: Counter/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] WebsiteCounterDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (id != inputModel.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.counter.Update(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Update failed");
        }
    }
}
