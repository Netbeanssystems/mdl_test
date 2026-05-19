using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class DocumentsController : Controller
    {
        private readonly IDataService _dataService;
        public DocumentsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET Countries
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.Documents.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Countries not found");
            return Ok(modelVms);
        }
        // GET Countries/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.Documents.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Country not found");
            return Ok(modelDto);
        }
        [HttpGet("{createdby}")]
        public async Task<IActionResult> Getdocuments([FromRoute] string createdby)
        {
            if (createdby == null) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.Documents.Getbycreatedby(createdby).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Country not found");
            return Ok(modelDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetURLsTiming()
        {
            var modelDto = await _dataService.Documents.GetURLsTiming().ConfigureAwait(false);
            if (modelDto == null) return NotFound("Country not found");
            return Ok(modelDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveURLsTiming()
        {
            var modelDto = await _dataService.Documents.GetActiveURLsTiming().ConfigureAwait(false);
            if (modelDto == null) return NotFound("No active upload windows found");
            return Ok(modelDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupedByURLsTiming()
        {
            var modelVms = await _dataService.Documents.GetGroupedByURLsTiming().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Documents not found");
            return Ok(modelVms);
        }

        [HttpGet]
        public async Task<IActionResult> GetByYearAndDescription(int? year, int? urlsTimingId)
        {
            var modelVms = await _dataService.Documents.GetByYearAndDescription(year, urlsTimingId).ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Documents not found");
            return Ok(modelVms);
        }

        [HttpGet]
        public async Task<IActionResult> GetByURLsTimingAndDateRange(int? urlsTimingId, string fromDate, string toDate)
        {
            DateTime? fromDateParsed = null;
            DateTime? toDateParsed = null;

            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fromDt))
                fromDateParsed = fromDt;

            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var toDt))
                toDateParsed = toDt;

            var modelVms = await _dataService.Documents.GetByURLsTimingAndDateRange(urlsTimingId, fromDateParsed, toDateParsed).ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Documents not found");
            return Ok(modelVms);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentsYears()
        {
            var years = await _dataService.Documents.GetDocumentsYears().ConfigureAwait(false);
            if (years == null || years.Count <= 0) return NotFound("No years found");
            return Ok(years);
        }

        // POST: Countries/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DocumentsDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Documents.Create(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }
        // PUT: Countries/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] DocumentsDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (id != inputModel.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Documents.Update(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Update failed");
        }
        // DELETE: Countries/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.Documents.Delete(id).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // POST: Countries/CreateRange
        [HttpPost]
        public async Task<IActionResult> CreateRange([FromBody] List<DocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count == 0) return BadRequest("Input not valid or null");
            var modelDtosTR = await _dataService.Documents.CreateRange(modelDtos).ConfigureAwait(false);
            if (modelDtosTR != null) return Ok(modelDtosTR);
            return BadRequest("Bulk insertion failed");
        }
        // POST: Countries/Upsert
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] List<DocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count == 0) return BadRequest("Input not valid or null");
            var modelDtosTR = await _dataService.Documents.Upsert(modelDtos).ConfigureAwait(false);
            if (modelDtosTR != null) return Ok(modelDtosTR);
            return BadRequest("Bulk upsertion failed");
        }
        // POST: Countries/DeleteRange
        [HttpPost]
        public async Task<IActionResult> DeleteRange([FromBody] List<DocumentsDTO> inputDtos)
        {
            if (inputDtos == null || inputDtos.Count == 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.Documents.DeleteRange(inputDtos).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Bulk deletion failed");
        }
        // GET GetDropdown
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDropdown()
        {
            var dropDownVms = await _dataService.Documents.GetDropdown().ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("Countries not found");
            return Ok(dropDownVms);
        }

        [HttpPost]
        public async Task<IActionResult> CreateURLsTiming([FromBody] URLsTimingDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Documents.CreateURLsTiming(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }

        [HttpGet]
        public async Task<IActionResult> GetClosedWindows()
        {
            var result = await _dataService.Documents.GetClosedWindows();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMultipleVisibility([FromBody] List<ClosedWindowsDTO> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest("The update list cannot be empty.");
            }

            var result = await _dataService.Documents.UpdateMultipleWindowsVisibility(dtos);

            if (!result)
            {
                return StatusCode(500, "An error occurred while updating the records.");
            }

            return NoContent(); // 204 No Content
        }


        [HttpGet]
        public async Task<ActionResult<List<DocumentsVM>>> GetVisibleDocuments()
        {
            var result = await _dataService.Documents.GetDocumentsIsShow();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentsList(int? urlsTimingId, string fromDate, string toDate)
        {
            DateTime? fromDateParsed = null;
            DateTime? toDateParsed = null;

            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fromDt))
                fromDateParsed = fromDt;

            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var toDt))
                toDateParsed = toDt;

            var modelVms = await _dataService.Documents.GetDocumentsList(urlsTimingId, fromDateParsed, toDateParsed).ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Documents not found");
            return Ok(modelVms);
        }

        [HttpPost]
        public async Task<IActionResult> LogDownload([FromBody] DocumentDownloadLogDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.DocumentName))
            {
                return BadRequest("Invalid log metrics structure.");
            }

            // Maps DTO data values to your database entity model instance
            var logEntity = new DocumentDownloadLog
            {
                DocumentName = dto.DocumentName,
                DownloadedBy = dto.DownloadedBy,
                DownloadedAt = dto.DownloadedAt,
                IpAddress = dto.IpAddress
            };

            // Assuming you follow a unit-of-work/data service pipeline pattern:
            var result = await _dataService.Documents.SaveDownloadLog(logEntity);

            if (!result)
            {
                return StatusCode(500, "An error occurred while tracking server audit records.");
            }

            return Ok();
        }
    }
}
