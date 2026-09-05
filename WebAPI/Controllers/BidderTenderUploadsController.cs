using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class BidderTenderUploadsController : Controller
    {
        private readonly IDataService _dataService;

        public BidderTenderUploadsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BidderTenderUploadsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest($"Model validation failed: {errors}");
            }
            try
            {
                var categoryDto = await _dataService.BidderTenderUpload.Add(modelDto).ConfigureAwait(false);
                if (categoryDto == null) return BadRequest("Create failed: Add operation returned null");
                return Ok(modelDto);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Exception in Create: {msg}");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
            {
            var categories = await _dataService.BidderTenderUpload.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("BidderTenderUploads not found");
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService.BidderTenderUpload.Get(id).ConfigureAwait(false);
            if (category == null) return NotFound("Home Page BidderTenderUpload Menu not found");
            return Ok(category);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] BidderTenderUploadsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest($"Invalid Id mismatch: route id {id} vs body id {modelDto.Id}");
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest($"Model validation failed: {errors}");
            }
            try
            {
                var categoryDto = await _dataService.BidderTenderUpload.Update(modelDto).ConfigureAwait(false);
                if (categoryDto == null) return BadRequest("Update failed: Update operation returned null");
                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Exception in Edit: {msg}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.BidderTenderUpload.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteCorrigendum([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var success = await _dataService.BidderTenderUpload.DeleteCorrigendum(id).ConfigureAwait(false);
            if (success) return Ok(true);
            return BadRequest("Delete failed");
        }
        [HttpPost]  
        public async Task<IActionResult> CheckTender([FromBody] BidderTenderUploadsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var model = await _dataService.BidderTenderUpload.CheckTender(modelDto).ConfigureAwait(false);
            if (model == null) return BadRequest("Failed to find tender");
            return Ok(model);
        }
        [HttpGet("{tenderNo}")]
        public async Task<IActionResult> GetByTenderNo([FromRoute] string tenderNo)
        {
            if (string.IsNullOrEmpty(tenderNo)) return BadRequest("Input not valid or null");
            var model = await _dataService.BidderTenderUpload.GetByTenderNo(tenderNo).ConfigureAwait(false);
            if (model == null) return BadRequest("Failed to find tender");
            return Ok(model);
        }
    }
}