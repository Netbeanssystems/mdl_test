using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class WebBankLogsController : Controller
    {
        private readonly IDataService _dataService;
        public WebBankLogsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET WebBankLogs
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.WebBankLogs.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Records not found");
            return Ok(modelVms.OrderByDescending(x => x.TimeStamp));
        }
        // GET WebBankLogs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVM([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelVm = await _dataService.WebBankLogs.GetVM(id).ConfigureAwait(false);
            if (modelVm == null) return NotFound("Record not found");
            return Ok(modelVm);
        }
        // GET WebBankLogs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.WebBankLogs.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Record not found");
            return Ok(modelDto);
        }
        // POST: WebBankLogs/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WebBankLogsDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.WebBankLogs.Create(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }
        // PUT: WebBankLogs/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] WebBankLogsDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (id != inputModel.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.WebBankLogs.Update(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Update failed");
        }
        // DELETE: WebBankLogs/Delete/5/false
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.WebBankLogs.Delete(id).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // POST: WebBankLogs/CreateRange
        [HttpPost]
        public async Task<IActionResult> CreateRange([FromBody] List<WebBankLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count == 0) return BadRequest("Input not valid or null");
            var modelDtosTR = await _dataService.WebBankLogs.CreateRange(modelDtos).ConfigureAwait(false);
            if (modelDtosTR != null) return Ok(modelDtosTR);
            return BadRequest("Bulk insertion failed");
        }
        // POST: WebBankLogs/Upsert
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] List<WebBankLogsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count == 0) return BadRequest("Input not valid or null");
            var modelDtosTR = await _dataService.WebBankLogs.Upsert(modelDtos).ConfigureAwait(false);
            if (modelDtosTR != null) return Ok(modelDtosTR);
            return BadRequest("Bulk upsertion failed");
        }
        // POST: WebBankLogs/DeleteRange
        [HttpPost]
        public async Task<IActionResult> DeleteRange([FromBody] List<WebBankLogsDTO> inputDtos)
        {
            if (inputDtos == null || inputDtos.Count == 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.WebBankLogs.DeleteRange(inputDtos).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Bulk deletion failed");
        }
    }
}
