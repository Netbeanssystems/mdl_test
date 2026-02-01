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
    public class TempBannerController : Controller
    {
        private readonly IDataService _dataService;
        public TempBannerController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var tempModelVMs = await _dataService._tempbanner.Get().ConfigureAwait(false);
            if (tempModelVMs == null || tempModelVMs.Count <= 0) return NotFound("Home Page Banners Menu not found");
            return Ok(tempModelVMs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var tempModelVM = await _dataService._tempbanner.Get(id).ConfigureAwait(false);
            if (tempModelVM == null) return NotFound("Home Page Banners Menu not found");
            return Ok(tempModelVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TempBannerDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempModelDTO = await _dataService._tempbanner.Add(modelDto).ConfigureAwait(false);
            if (tempModelDTO == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] TempBannerDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempModelDTO = await _dataService._tempbanner.Update(modelDto).ConfigureAwait(false);
            if (tempModelDTO == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService._tempbanner.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAudit([FromBody] TempBannerDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ModelDTO = await _dataService._tempbanner.CreateAudit(modelDto).ConfigureAwait(false);
            if (ModelDTO == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }

        [HttpGet("{status}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByAction([FromRoute] string status)
        {
            if (status == null) return BadRequest("Input not valid or null");
            var TempMenu = await _dataService._tempbanner.GetByAction(status).ConfigureAwait(false);
            if (TempMenu == null) return NotFound(" Menu Heading Data not found");
            return Ok(TempMenu);
        }
    }
}