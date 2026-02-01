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
    public class TempMediaController : Controller
    {
        private readonly IDataService _dataService;

        public TempMediaController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tempNewProjects = await _dataService._tempmedia.Get().ConfigureAwait(false);
            if (tempNewProjects == null || tempNewProjects.Count <= 0) return NotFound("Press Release not found");
            return Ok(tempNewProjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var tempNewProject = await _dataService._tempmedia.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return NotFound("New Projects not found");
            return Ok(tempNewProject);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TempMediaDTO TemppressReleaseDTO)
        {
            if (TemppressReleaseDTO == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempNewProjectDto = await _dataService._tempmedia.Add(TemppressReleaseDTO).ConfigureAwait(false);
            if (tempNewProjectDto == null) return BadRequest("Create failed");
            return Ok(TemppressReleaseDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] TempMediaDTO TemppressReleaseDTO)
        {
            if (TemppressReleaseDTO == null) return BadRequest("Input not valid or null");
            if (id != TemppressReleaseDTO.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempNewProjectDto = await _dataService._tempmedia.Update(TemppressReleaseDTO).ConfigureAwait(false);
            if (tempNewProjectDto == null) return BadRequest("Update failed");
            return Ok(TemppressReleaseDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService._tempmedia.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAudit([FromBody] TempMediaDTO TemppressReleaseDTO)
        {
            if (TemppressReleaseDTO == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempTempNewProjectDto = await _dataService._tempmedia.CreateAudit(TemppressReleaseDTO).ConfigureAwait(false);
            if (tempTempNewProjectDto == null) return BadRequest("Create failed");
            return Ok(TemppressReleaseDTO);
        }

        [HttpGet("{status}")]
        public async Task<IActionResult> GetByAction([FromRoute] string status)
        {
            if (status == null) return BadRequest("Input not valid or null");
            var TempPress = await _dataService._tempmedia.GetByAction(status).ConfigureAwait(false);
            if (TempPress == null) return NotFound(" Petrol Data not found");
            return Ok(TempPress);
        }
    }
}