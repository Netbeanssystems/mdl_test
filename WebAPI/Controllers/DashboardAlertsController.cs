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
    public class DashboardAlertsController : Controller
    {
        private readonly IDataService _dataService;
        public DashboardAlertsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET DashboardAlerts
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.DashboardAlerts.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Dashboard Alerts not found");
            return Ok(modelVms);
        }
        // GET DashboardAlerts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.DashboardAlerts.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Dashboard Alerts not found");
            return Ok(modelDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.DashboardAlerts.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Dashboard Alerts not found");
            return Ok(modelDto);
        }
        // POST: DashboardAlerts/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DashboardAlertsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var IsDuplicate = await _dataService.DashboardAlerts.CheckDuplicate(argModelDto).ConfigureAwait(false);
            if (IsDuplicate) return BadRequest("This record already exists");
            var modelDto = await _dataService.DashboardAlerts.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // PUT: DashboardAlerts/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] DashboardAlertsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (id != argModelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.DashboardAlerts.Update(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: DashboardAlerts/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.DashboardAlerts.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // GET DashboardAlerts/GetActiveAlert
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetActiveAlert()
        {
            var modelVms = await _dataService.DashboardAlerts.GetActiveAlert().ConfigureAwait(false);
            if (modelVms == null) return NotFound("Dashboard Alert not found");
            return Ok(modelVms);
        }
        // GET DashboardAlerts/GetDropdown
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDropdown()
        {
            var dropDownVms = await _dataService.DashboardAlerts.GetDropdown().ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("Values not found");
            return Ok(dropDownVms);
        }
    }
}
