using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class NotificationDetailsController : Controller
    {
        private readonly IDataService _dataService;
        public NotificationDetailsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET NotificationDetails
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.NotificationDetails.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Notification details not found");
            return Ok(modelVms);
        }
        // GET NotificationDetails/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.NotificationDetails.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Notification details not found");
            return Ok(modelDto);
        }
        // GET NotificationDetails/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVM([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelVm = await _dataService.NotificationDetails.GetVM(id).ConfigureAwait(false);
            if (modelVm == null) return NotFound("Notification details not found");
            return Ok(modelVm);
        }
        // POST: NotificationDetails/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationDetailsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.NotificationDetails.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // PUT: NotificationDetails/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] NotificationDetailsDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (id != argModelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.NotificationDetails.Update(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: NotificationDetails/Delete/5/false
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.NotificationDetails.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // GET NotificationDetails/GetNotificationDetailsWithAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var modelVms = await _dataService.NotificationDetails.GetAll().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("NotificationDetails not found");
            return Ok(modelVms);
        }
        // GET NotificationDetails/GetByUser
        [HttpGet("{unm}")]
        public async Task<IActionResult> GetByUser([FromRoute] string unm)
        {
            var modelVms = await _dataService.NotificationDetails.GetByUser(unm).ConfigureAwait(false);
            return Ok(modelVms);
        }
        // GET NotificationDetails/GetByNotificationId
        [HttpGet("{nid}/{unm}")]
        public async Task<IActionResult> GetByNotificationId([FromRoute] int nid, [FromRoute] string unm)
        {
            var modelVms = await _dataService.NotificationDetails.GetByNotificationId(nid, unm).ConfigureAwait(false);
            return Ok(modelVms);
        }
        //// DELETE: NotificationDetails/Delete/5/Admin001/false
        //[HttpDelete("{id}/{unm}/{hard}")]
        //public async Task<IActionResult> Delete([FromRoute] int id, [FromRoute] string unm, [FromRoute] bool hard)
        //{
        //    if (id <= 0) return BadRequest("Input not valid or null");
        //    var rowsAffected = await _dataService.NotificationDetails.Delete(id, unm, hard).ConfigureAwait(false);
        //    if (rowsAffected > 0) return Ok(rowsAffected);
        //    return BadRequest("Delete failed. There might be active child records.");
        //}
        // GET NotificationDetails/CreateFulfilments
        [HttpGet("{modelStr}")]
        public async Task<IActionResult> DeleteSelected([FromRoute] string modelStr)
        {
            var rowsChanged = 0;
            if (string.IsNullOrEmpty(modelStr)) return BadRequest("Input not valid or null");
            var idsStr = JsonConvert.DeserializeObject<List<string>>(modelStr);
            if (idsStr != null)
                foreach (var item in idsStr)
                {
                    var id = Convert.ToInt32(item.Split('-')[2]);
                    rowsChanged += await _dataService.NotificationDetails.Delete(id).ConfigureAwait(false);
                }

            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Notification(s) couldn't be deleted");
        }
    }
}
