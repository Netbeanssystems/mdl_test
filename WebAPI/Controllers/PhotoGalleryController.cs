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
    public class PhotoGalleryController : Controller
    {
        private readonly IDataService _dataService;

        public PhotoGalleryController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var newProjects = _dataService.PhotoGallery.Get();
            if (newProjects == null || newProjects.Count <= 0) return NotFound("PhotoGallery not found");
            return Ok(newProjects);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetbyPriorty()
        {
            var newProjects = await _dataService.PhotoGallery.GetbyPriorty().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return NotFound("PhotoGallery not found");
            return Ok(newProjects);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetbyPriortyForAwardAccolades()
        {
            var newProjects = await _dataService.PhotoGallery.GetbyPriortyForAwardAccolades().ConfigureAwait(false);
            if (newProjects == null || newProjects.Count <= 0) return NotFound("Award Accolades not found");
            return Ok(newProjects);
        }

        [AllowAnonymous]
        [HttpGet("{Key}")]
        public async Task<IActionResult> GetCategory([FromRoute] string Key)
        {
            if (Key == null) return BadRequest("Input not valid or null");
            var category = await _dataService.PhotoGallery.GetCategory(Key).ConfigureAwait(false);
            if (category == null) return NotFound("PhotoGallery not found");
            return Ok(category);
        }

        [AllowAnonymous]
        [HttpGet("{Key}/{PageNo}/{PageSize}")]
        public async Task<IActionResult> Getorder([FromRoute] string Key, [FromRoute] int PageNo, [FromRoute] int PageSize)
        {
            var pressrelease = await _dataService.PhotoGallery.Getorder(Key, PageNo, PageSize).ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("PhotoGallery  not found");
            return Ok(pressrelease);
        }

        [AllowAnonymous]
        [HttpGet("{Key}/{PageNo}/{PageSize}")]
        public async Task<IActionResult> GetorderHindi([FromRoute] string Key, [FromRoute] int PageNo, [FromRoute] int PageSize)
        {
            var pressrelease = await _dataService.PhotoGallery.GetorderHindi(Key, PageNo, PageSize).ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("PhotoGallery  not found");
            return Ok(pressrelease);
        }


        [AllowAnonymous]
        [HttpGet("{Key}/{PageNo}/{PageSize}")]
        public async Task<IActionResult> GetorderPriortyForAwardAccolades([FromRoute] string Key, [FromRoute] int PageNo, [FromRoute] int PageSize)
        {
            var pressrelease = await _dataService.PhotoGallery.GetorderPriortyForAwardAccolades(Key, PageNo, PageSize).ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("Award-Gallery not found");
            return Ok(pressrelease);
        }

        [AllowAnonymous]
        [HttpGet("{Key}/{PageNo}/{PageSize}")]
        public async Task<IActionResult> GetorderPriortyForAwardAccoladeshindi([FromRoute] string Key, [FromRoute] int PageNo, [FromRoute] int PageSize)
        {
            var pressrelease = await _dataService.PhotoGallery.GetorderPriortyForAwardAccoladeshindi(Key, PageNo, PageSize).ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("Award-Gallery Hindi not found");
            return Ok(pressrelease);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhotoGalleryDTO videoDto)
        {
            if (videoDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newProjectDto = await _dataService.PhotoGallery.Add(videoDto).ConfigureAwait(false);
            if (newProjectDto == null) return BadRequest("Create failed");
            return Ok(videoDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] PhotoGalleryDTO videoDto)
        {
            if (videoDto == null) return BadRequest("Input not valid or null");
            if (id != videoDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newProjectDto = await _dataService.PhotoGallery.Update(videoDto).ConfigureAwait(false);
            if (newProjectDto == null) return BadRequest("Update failed");
            return Ok(videoDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.PhotoGallery.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var mheading = await _dataService.PhotoGallery.Get(id).ConfigureAwait(false);
            if (mheading == null) return NotFound("Menu headings not found");
            return Ok(mheading);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetTopEvents()
        {
            var newProjects = _dataService.PhotoGallery.GetTopEvents();
            if (newProjects == null || newProjects.Count <= 0) return NotFound("Events not found");
            return Ok(newProjects);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetYears()
        {
            var newProjects = _dataService.PhotoGallery.GetYears();
            if (newProjects == null || newProjects.Count <= 0) return NotFound("Events not found");
            return Ok(newProjects);
        }

        [AllowAnonymous]
        [HttpGet("{year}")]
        public IActionResult GetEventsByYear([FromRoute] int year)
        {
            var newProjects = _dataService.PhotoGallery.GetEventsByYear(year);
            if (newProjects == null || newProjects.Count <= 0) return NotFound("Events not found");
            return Ok(newProjects);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAllEvents()
        {
            var newProjects = _dataService.PhotoGallery.GetEventsByYear(0);
            if (newProjects == null || newProjects.Count <= 0) return NotFound("Events not found");
            return Ok(newProjects);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetPhotos([FromRoute] int id)
        {
            var newProjects = _dataService.PhotoGallery.GetPhotos(id);
            if (newProjects == null || newProjects.Count <= 0) return NotFound("Events not found");
            return Ok(newProjects);
        }
    }
}
