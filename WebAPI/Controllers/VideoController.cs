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
    public class VideoController : Controller
    {
        private readonly IDataService _dataService;

        public VideoController(IDataService dataService)
        {
            _dataService = dataService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var newProjects = await _dataService.Video.Get().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return NotFound("Video not found");
            return Ok(newProjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var newProject = await _dataService.Video.Get(id).ConfigureAwait(false);
            if (newProject == null) return NotFound("Video not found");
            return Ok(newProject);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetLastest()
        //{
        //    var newProjects = await _dataService.Video.Get().ConfigureAwait(false);
        //    if (newProjects == null || newProjects.Count <= 0) return NotFound("Fakewebsite not found");
        //    return Ok(newProjects.LastOrDefault());
        //}


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VideoDTO videoDto)
        {
            if (videoDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newProjectDto = await _dataService.Video.Add(videoDto).ConfigureAwait(false);
            if (newProjectDto == null) return BadRequest("Create failed");
            return Ok(videoDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] VideoDTO videoDto)
        {
            if (videoDto == null) return BadRequest("Input not valid or null");
            if (id != videoDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newProjectDto = await _dataService.Video.Update(videoDto).ConfigureAwait(false);
            if (newProjectDto == null) return BadRequest("Update failed");
            return Ok(videoDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.Video.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }
    }
}
