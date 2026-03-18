using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AcademicYearsController : Controller
    {
        private readonly IDataService _dataService;
        public AcademicYearsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET AcademicYears
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.AcademicYears.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Academic Years not found");
            return Ok(modelVms);
        }
        // GET AcademicYears/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.AcademicYears.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Academic Years not found");
            return Ok(modelDto);
        }
        // POST: AcademicYears/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AcademicYearsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var IsDuplicate = await _dataService.AcademicYears.CheckDuplicate(argModelDto).ConfigureAwait(false);
            if (IsDuplicate) return BadRequest("This record already exists");
            var modelDto = await _dataService.AcademicYears.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // PUT: AcademicYears/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] AcademicYearsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (id != argModelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.AcademicYears.Update(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: AcademicYears/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.AcademicYears.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // GET AcademicYears/GetWithAll
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetWithAll()
        {
            var modelVms = await _dataService.AcademicYears.GetWithAll().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Academic years not found");
            return Ok(modelVms);
        }
        // GET AcademicYears/GetDropdown
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDropdown()
        {
            var dropDownVms = await _dataService.AcademicYears.GetDropdown().ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("Values not found");
            return Ok(dropDownVms);
        }
    }
}
