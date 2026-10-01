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
    public class TempVideoController : Controller
    {
        private readonly IDataService _dataService;

        public TempVideoController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var tempNewProjects = await _dataService.TempVideo.Get().ConfigureAwait(false);
            if (tempNewProjects == null || tempNewProjects.Count <= 0) return NotFound("Video not found");
            return Ok(tempNewProjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var tempNewProject = await _dataService.TempVideo.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return NotFound("Video not found");
            return Ok(tempNewProject);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TempVideoDTO tempvideoDto)
        {
            if (tempvideoDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempNewProjectDto = await _dataService.TempVideo.Add(tempvideoDto).ConfigureAwait(false);
            if (tempNewProjectDto == null) return BadRequest("Create failed");
            return Ok(tempvideoDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] TempVideoDTO tempvideoDto)
        {
            if (tempvideoDto == null) return BadRequest("Input not valid or null");
            if (id != tempvideoDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempNewProjectDto = await _dataService.TempVideo.Update(tempvideoDto).ConfigureAwait(false);
            if (tempNewProjectDto == null) return BadRequest("Update failed");
            return Ok(tempvideoDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.TempVideo.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }


        [HttpPost]
        public async Task<IActionResult> CreateAudit([FromBody] TempVideoDTO tempvideoDto)
        {
            if (tempvideoDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempTempNewProjectDto = await _dataService.TempVideo.CreateAudit(tempvideoDto).ConfigureAwait(false);
            if (tempTempNewProjectDto == null) return BadRequest("Create failed");
            return Ok(tempvideoDto);
        }

        [HttpGet("{status}")]
        public async Task<IActionResult> GetByAction([FromRoute] string status)
        {
            if (status == null) return BadRequest("Input not valid or null");
            var TempPetrol = await _dataService.TempVideo.GetByAction(status).ConfigureAwait(false);
            if (TempPetrol == null) return NotFound(" Video not found");
            return Ok(TempPetrol);
        }
    }
}
