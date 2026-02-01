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
    public class TempWhatsNewController : Controller
    {
        private readonly IDataService _dataService;

        public TempWhatsNewController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var tempNewProjects = await _dataService._TempwhatsNew.Get().ConfigureAwait(false);
            if (tempNewProjects == null || tempNewProjects.Count <= 0) return NotFound("News not found");
            return Ok(tempNewProjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var tempNewProject = await _dataService._TempwhatsNew.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return NotFound("News not found");
            return Ok(tempNewProject);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TempWhatsNewDTO tempnewsDto)
        {
            if (tempnewsDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempNewProjectDto = await _dataService._TempwhatsNew.Add(tempnewsDto).ConfigureAwait(false);
            if (tempNewProjectDto == null) return BadRequest("Create failed");
            return Ok(tempnewsDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] TempWhatsNewDTO tempnewsDto)
        {
            if (tempnewsDto == null) return BadRequest("Input not valid or null");
            if (id != tempnewsDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempNewProjectDto = await _dataService._TempwhatsNew.Update(tempnewsDto).ConfigureAwait(false);
            if (tempNewProjectDto == null) return BadRequest("Update failed");
            return Ok(tempnewsDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService._TempwhatsNew.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAudit([FromBody] TempWhatsNewDTO tempnewsDto)
        {
            if (tempnewsDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempTempNewProjectDto = await _dataService._TempwhatsNew.CreateAudit(tempnewsDto).ConfigureAwait(false);
            if (tempTempNewProjectDto == null) return BadRequest("Create failed");
            return Ok(tempnewsDto);
        }
        [HttpGet("{status}")]
        public async Task<IActionResult> GetByAction([FromRoute] string status)
        {
            if (status == null) return BadRequest("Input not valid or null");
            var TempNews = await _dataService._TempwhatsNew.GetByAction(status).ConfigureAwait(false);
            if (TempNews == null) return NotFound(" News Data not found");
            return Ok(TempNews);
        }
    }
}