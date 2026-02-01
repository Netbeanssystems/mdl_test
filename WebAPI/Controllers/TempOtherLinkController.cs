using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class TempOtherLinkController : Controller
    {
        private readonly IDataService _dataService;

        public TempOtherLinkController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET 
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var tempModelVMs = await _dataService.TempOtherLinkHeading.Get().ConfigureAwait(false);
            if (tempModelVMs == null || tempModelVMs.Count <= 0) return NotFound("headings not found");
            return Ok(tempModelVMs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var tempModelVM = await _dataService.TempOtherLinkHeading.Get(id).ConfigureAwait(false);
            if (tempModelVM == null) return NotFound(" headings not found");
            return Ok(tempModelVM);
        }

        // create 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TempOtherLinkHeadingDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempModelDTO = await _dataService.TempOtherLinkHeading.Add(modelDto).ConfigureAwait(false);
            if (tempModelDTO == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] TempOtherLinkHeadingDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempModelDTO = await _dataService.TempOtherLinkHeading.Update(modelDto).ConfigureAwait(false);
            if (tempModelDTO == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.TempOtherLinkHeading.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAudit([FromBody] TempOtherLinkHeadingDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ModelDTO = await _dataService.TempOtherLinkHeading.CreateAudit(modelDto).ConfigureAwait(false);
            if (ModelDTO == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        [HttpGet("{status}")]
        public async Task<IActionResult> GetByAction([FromRoute] string status)
        {
            if (status == null) return BadRequest("Input not valid or null");
            var Tempproduct = await _dataService.TempOtherLinkHeading.GetByAction(status).ConfigureAwait(false);
            if (Tempproduct == null) return NotFound("Data not found");
            return Ok(Tempproduct);
        }
    }

}