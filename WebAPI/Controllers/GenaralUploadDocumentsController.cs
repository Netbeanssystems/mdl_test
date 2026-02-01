using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class GenaralUploadDocumentsController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly IDataService _dataService;
        public GenaralUploadDocumentsController(IDataService dataService, IWebHostEnvironment env)
        {
            _dataService = dataService;
            _env = env;
        }
        // GET Countries
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.GenaralUploadDocuments.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Countries not found");
            return Ok(modelVms);
        }

        public IActionResult DownloadFile(string id)
        {
            var fileRecord = id;
            if (fileRecord == null)
                return NotFound("File not found");

            var filePath = Path.Combine(_env.WebRootPath, "Bidder", "img", "UploadedFiles", "GeneralUpload", fileRecord);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileRecord);
        }

        // GET Countries/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.GenaralUploadDocuments.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Country not found");
            return Ok(modelDto);
        }
        [HttpGet("{createdby}")]
        public async Task<IActionResult> Getdocuments([FromRoute] string createdby)
        {
            if (createdby == null) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.GenaralUploadDocuments.Getbycreatedby(createdby).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Country not found");
            return Ok(modelDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetURLsTiming()
        {
            var modelDto = await _dataService.GenaralUploadDocuments.GetURLsTiming().ConfigureAwait(false);
            if (modelDto == null) return NotFound("Country not found");
            return Ok(modelDto);
        }

        // POST: Countries/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GenaralUploadDocumentsDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.GenaralUploadDocuments.Create(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }
        // PUT: Countries/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] GenaralUploadDocumentsDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (id != inputModel.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.GenaralUploadDocuments.Update(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Update failed");
        }
        // DELETE: Countries/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.GenaralUploadDocuments.Delete(id).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // POST: Countries/CreateRange
        [HttpPost]
        public async Task<IActionResult> CreateRange([FromBody] List<GenaralUploadDocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count == 0) return BadRequest("Input not valid or null");
            var modelDtosTR = await _dataService.GenaralUploadDocuments.CreateRange(modelDtos).ConfigureAwait(false);
            if (modelDtosTR != null) return Ok(modelDtosTR);
            return BadRequest("Bulk insertion failed");
        }
        // POST: Countries/Upsert
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] List<GenaralUploadDocumentsDTO> modelDtos)
        {
            if (modelDtos == null || modelDtos.Count == 0) return BadRequest("Input not valid or null");
            var modelDtosTR = await _dataService.GenaralUploadDocuments.Upsert(modelDtos).ConfigureAwait(false);
            if (modelDtosTR != null) return Ok(modelDtosTR);
            return BadRequest("Bulk upsertion failed");
        }
        // POST: Countries/DeleteRange
        [HttpPost]
        public async Task<IActionResult> DeleteRange([FromBody] List<GenaralUploadDocumentsDTO> inputDtos)
        {
            if (inputDtos == null || inputDtos.Count == 0) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.GenaralUploadDocuments.DeleteRange(inputDtos).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Bulk deletion failed");
        }
        // GET GetDropdown
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDropdown()
        {
            var dropDownVms = await _dataService.GenaralUploadDocuments.GetDropdown().ConfigureAwait(false);
            if (dropDownVms == null || dropDownVms.Count <= 0) return NotFound("Countries not found");
            return Ok(dropDownVms);
        }

        [HttpPost]
        public async Task<IActionResult> CreateURLsTiming([FromBody] GeneraluploadURLDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.GenaralUploadDocuments.CreateURLsTiming(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }
    
    }
}
