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
    public class TempPhotoGalleryController : Controller
    {
        private readonly IDataService _dataService;
        public TempPhotoGalleryController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var tempNewProjects = await _dataService.TempPhotoGallery.Get().ConfigureAwait(false);
            if (tempNewProjects == null || tempNewProjects.Count <= 0) return NotFound("TempPhotoGallery not found");
            return Ok(tempNewProjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var tempNewProject = await _dataService.TempPhotoGallery.Get(id).ConfigureAwait(false);
            if (tempNewProject == null) return NotFound("TempPhotoGallery not found");
            return Ok(tempNewProject);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TempPhotoGalleryDTO tempvideoDto)
        {
            if (tempvideoDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempNewProjectDto = await _dataService.TempPhotoGallery.Add(tempvideoDto).ConfigureAwait(false);
            if (tempNewProjectDto == null) return BadRequest("Create failed");
            return Ok(tempvideoDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] TempPhotoGalleryDTO tempvideoDto)
        {
            if (tempvideoDto == null) return BadRequest("Input not valid or null");
            if (id != tempvideoDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempNewProjectDto = await _dataService.TempPhotoGallery.Update(tempvideoDto).ConfigureAwait(false);
            if (tempNewProjectDto == null) return BadRequest("Update failed");
            return Ok(tempvideoDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.TempPhotoGallery.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }


        [HttpPost]
        public async Task<IActionResult> CreateAudit([FromBody] TempPhotoGalleryDTO tempvideoDto)
        {
            if (tempvideoDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var tempTempNewProjectDto = await _dataService.TempPhotoGallery.CreateAudit(tempvideoDto).ConfigureAwait(false);
            if (tempTempNewProjectDto == null) return BadRequest("Create failed");
            return Ok(tempvideoDto);
        }

        [HttpGet("{status}")]
        public async Task<IActionResult> GetByAction([FromRoute] string status)
        {
            if (status == null) return BadRequest("Input not valid or null");
            var TempPetrol = await _dataService.TempPhotoGallery.GetByAction(status).ConfigureAwait(false);
            if (TempPetrol == null) return NotFound(" TempPhotoGallery not found");
            return Ok(TempPetrol);
        }
    }
}
