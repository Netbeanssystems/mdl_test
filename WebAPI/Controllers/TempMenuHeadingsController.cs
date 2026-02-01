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
    public class TempMenuHeadingsController : Controller
    {
        private readonly IDataService _dataService;

        public TempMenuHeadingsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET TempAnnualReport
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var tempModelVMs = await _dataService.TempMenuHeadings.Get().ConfigureAwait(false);
            if (tempModelVMs == null || tempModelVMs.Count <= 0) return NotFound("Menu headings not found");
            return Ok(tempModelVMs);
        }
        // GET TempAnnualReport/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var tempModelVM = await _dataService.TempMenuHeadings.Get(id).ConfigureAwait(false);
            if (tempModelVM == null) return NotFound("Menu headings not found");
            return Ok(tempModelVM);
        }
        // POST: TempAnnualReport/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TempMenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempModelDTO = await _dataService.TempMenuHeadings.Add(modelDto).ConfigureAwait(false);
            if (tempModelDTO == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // PUT: TempAnnualReport/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] TempMenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempModelDTO = await _dataService.TempMenuHeadings.Update(modelDto).ConfigureAwait(false);
            if (tempModelDTO == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: TempAnnualReport/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.TempMenuHeadings.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }
        // POST: TempAnnualReport/CreateAudit
        [HttpPost]
        public async Task<IActionResult> CreateAudit([FromBody] TempMenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ModelDTO = await _dataService.TempMenuHeadings.CreateAudit(modelDto).ConfigureAwait(false);
            if (ModelDTO == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }

        [HttpGet("{status}")]
        public async Task<IActionResult> GetByAction([FromRoute] string status)
        {
            if (status == null) return BadRequest("Input not valid or null");
            var TempMenu = await _dataService.TempMenuHeadings.GetByAction(status).ConfigureAwait(false);
            if (TempMenu == null) return NotFound(" Menu Heading Data not found");
            return Ok(TempMenu);
        }
        [HttpGet("{status}")]
        public async Task<IActionResult> GetByActionupdate([FromRoute] string status)
        {
            if (status == null) return BadRequest("Input not valid or null");
            var TempMenu = await _dataService.TempMenuHeadings.GetByActionupdate(status).ConfigureAwait(false);
            if (TempMenu == null) return NotFound(" Menu Heading Data not found");
            return Ok(TempMenu);
        }
    }
}