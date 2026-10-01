using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class NotificationsController : Controller
    {
        private readonly IDataService _dataService;
        public NotificationsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET Notifications
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.Notifications.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Notifications not found");
            return Ok(modelVms);
        }
        // GET Notifications/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.Notifications.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Notifications not found");
            return Ok(modelDto);
        }
        // GET Notifications/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVM([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelVm = await _dataService.Notifications.GetVM(id).ConfigureAwait(false);
            if (modelVm == null) return NotFound("Notifications not found");
            return Ok(modelVm);
        }
        // POST: Notifications/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Notifications.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // PUT: Notifications/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] NotificationsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (id != argModelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Notifications.Update(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: Notifications/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.Notifications.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // GET Notifications/GetNotificationsWithAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var modelVms = await _dataService.Notifications.GetAll().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Notifications not found");
            return Ok(modelVms);
        }
        // GET Notifications/GetByUser
        [HttpGet("{unm}")]
        public async Task<IActionResult> GetByUser([FromRoute] string unm)
        {
            var modelVms = await _dataService.Notifications.GetByUser(unm).ConfigureAwait(false);
            return Ok(modelVms);
        }
        // POST: Notifications/Notify
        [HttpPost]
        public async Task<IActionResult> Notify([FromBody] NotificationVM argModelVm)
        {
            if (argModelVm == null) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.Notifications.Notify(argModelVm).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
    }
}
