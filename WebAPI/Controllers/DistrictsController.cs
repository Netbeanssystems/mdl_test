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
    public class DistrictsController : Controller
    {
        private readonly IDataService _dataService;
        public DistrictsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET Districts
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.Districts.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Districts not found");
            return Ok(modelVms);
        }
        // GET Districts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.Districts.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Districts not found");
            return Ok(modelDto);
        }
        // POST: Districts/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DistrictsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Districts.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // PUT: Districts/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] DistrictsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (id != argModelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Districts.Update(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: Districts/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.Districts.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed. There might be active child records.");
        }
        [AllowAnonymous]
        // GET GetDistrictCode
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistrictCode([FromRoute] int id)
        {
            var code = await _dataService.Districts.GetDistrictCode(id).ConfigureAwait(false);
            if (code == null) return NotFound("Districts not found");
            return Ok(code);
        }
        // GET GetDropdown
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDropdown()
        {
            var dropDownVms = await _dataService.Districts.GetDropdown().ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("District not found");
            return Ok(dropDownVms);
        }
        // GET GetDropdownByState
        [AllowAnonymous]
        [HttpGet("{stateid}")]
        public async Task<IActionResult> GetDropdownByState([FromRoute] int stateid)
        {
            if (stateid <= 0) return Ok(null);
            var dropDownVms = await _dataService.Districts.GetDropdownByState(stateid).ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("Districts not found");
            return Ok(dropDownVms);
        }
    }
}
