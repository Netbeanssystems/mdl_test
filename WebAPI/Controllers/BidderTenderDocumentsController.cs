using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class BidderTenderDocumentsController : Controller
    {
        private readonly IDataService _dataService;

        public BidderTenderDocumentsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BidderTenderDocumentsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService.BidderTenderDocuments.Add(modelDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var categories = await _dataService.BidderTenderDocuments.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Documents not found");
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService.BidderTenderDocuments.Get(id).ConfigureAwait(false);
            if (category == null) return NotFound("Home Page BidderTenderDocuments Menu not found");
            return Ok(category);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] BidderTenderDocumentsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService.BidderTenderDocuments.Update(modelDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Update failed");
            return Ok(categoryDto);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.BidderTenderDocuments.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }
    }
}