using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class StatesController : Controller
    {
        private readonly IDataService _dataService;
        public StatesController(IDataService dataService)
        {
            _dataService = dataService;
        }
        [AllowAnonymous]
        // GET States
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.States.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("States not found");
            return Ok(modelVms);
        }
        // GET States/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.States.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("State not found");
            return Ok(modelDto);
        }
        // POST: States/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StatesDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.States.Create(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }
        // PUT: States/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] StatesDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (id != inputModel.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.States.Update(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Update failed");
        }
        // DELETE: States/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.States.Delete(id).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // POST: States/CreateRange
        [HttpPost]
        public async Task<IActionResult> CreateRange([FromBody] List<StatesDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count == 0) return BadRequest("Input not valid or null");
            var modelDtosTR = await _dataService.States.CreateRange(modelDtos).ConfigureAwait(false);
            if (modelDtosTR != null) return Ok(modelDtosTR);
            return BadRequest("Bulk insertion failed");
        }
        // POST: States/Upsert
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] List<StatesDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count == 0) return BadRequest("Input not valid or null");
            var modelDtosTR = await _dataService.States.Upsert(modelDtos).ConfigureAwait(false);
            if (modelDtosTR != null) return Ok(modelDtosTR);
            return BadRequest("Bulk upsertion failed");
        }
        // POST: States/DeleteRange
        [HttpPost]
        public async Task<IActionResult> DeleteRange([FromBody] List<StatesDTO> inputDtos)
        {
            if (inputDtos == null || inputDtos.Count == 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.States.DeleteRange(inputDtos).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Bulk deletion failed");
        }
        [AllowAnonymous]
        // GET GetDropdown
        [HttpGet]
        public async Task<IActionResult> GetDropdown()
        {
            var dropDownVms = await _dataService.States.GetDropdown().ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("States not found");
            return Ok(dropDownVms);
        }
        [AllowAnonymous]
        // GET GetDropdownByCountry
        [HttpGet("{countryid}")]
        public async Task<IActionResult> GetDropdownByCountry([FromRoute] int countryid)
        {
            if (countryid <= 0) return Ok(null);
            var dropDownVms = await _dataService.States.GetDropdownByCountry(countryid).ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("States not found");
            return Ok(dropDownVms);
        }
    }
}
