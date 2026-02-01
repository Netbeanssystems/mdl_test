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
    public class EventsController : Controller
    {
        private readonly IDataService _dataService;
        public EventsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var modelVms = await _dataService.events.Get().ConfigureAwait(false);
        //    if (modelVms == null || modelVms.Count <= 0) return NotFound("States not found");
        //    return Ok(modelVms);
        //}
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var modelVms = await _dataService.events.GetAllEvents().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("States not found");
            return Ok(modelVms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.events.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("State not found");
            return Ok(modelDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.events.Create(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] EventDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (id != inputModel.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.events.Update(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Update failed");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.events.Delete(id).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Delete failed. There might be active child records.");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDropdown()
        {
            var dropDownVms = await _dataService.events.GetDropdown().ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("States not found");
            return Ok(dropDownVms);
        }

        [AllowAnonymous]
        [HttpGet("{countryid}")]
        public async Task<IActionResult> GetDropdownById([FromRoute] int countryid)
        {
            if (countryid <= 0) return Ok(null);
            var dropDownVms = await _dataService.events.GetDropdownById(countryid).ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("States not found");
            return Ok(dropDownVms);
        }
    }
}
