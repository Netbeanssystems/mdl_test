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
    public class MediaController : Controller
    {
        private readonly IDataService _dataService;

        public MediaController(IDataService dataService)
        {
            _dataService = dataService;
        }

        //--------------------------------Old
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var pressrelease = await _dataService._media.Get().ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("Press Release  not found");
            return Ok(pressrelease);
        }

        //-------------------------Karn 11Dec 2023-----------
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllMedia()
        {
            var pressrelease = await _dataService._media.GetAllMedia().ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("Press Release  not found");
            return Ok(pressrelease);
        }

        [AllowAnonymous]
        [HttpGet("{Key}/{PageNo}/{PageSize}")]
        public async Task<IActionResult> Getorder([FromRoute] string Key, [FromRoute] int PageNo, [FromRoute] int PageSize)
        {
            var pressrelease = await _dataService._media.Getorder(Key, PageNo, PageSize).ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("Data  not found");
            return Ok(pressrelease);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getnews()
        {
            var pressrelease = await _dataService._media.Getnews().ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("Press Release  not found");
            return Ok(pressrelease);
        }
        [AllowAnonymous]
        [HttpGet("{PageNo}/{PageSize}/{Minpage}")]
        public async Task<IActionResult> Getorderdesc([FromRoute] int PageNo, [FromRoute] int PageSize, [FromRoute] int Minpage)
        {
            var pressrelease = await _dataService._media.Getorderdesc(PageNo, PageSize, Minpage).ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("Press Release  not found");
            return Ok(pressrelease);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getorderdescfull()
        {
            var pressrelease = await _dataService._media.Getorderdescfull().ConfigureAwait(false);
            if (pressrelease == null || pressrelease.Count <= 0) return NotFound("Press Release  not found");
            return Ok(pressrelease);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var pressrelease = await _dataService._media.Get(id).ConfigureAwait(false);
            if (pressrelease == null) return NotFound("Press Release not found");
            return Ok(pressrelease);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MediaDTO pressReleaseDTO)
        {
            if (pressReleaseDTO == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newProjectDto = await _dataService._media.Add(pressReleaseDTO).ConfigureAwait(false);
            if (newProjectDto == null) return BadRequest("Create failed");
            return Ok(pressReleaseDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] MediaDTO pressReleaseDTO)
        {
            if (pressReleaseDTO == null) return BadRequest("Input not valid or null");
            if (id != pressReleaseDTO.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newProjectDto = await _dataService._media.Update(pressReleaseDTO).ConfigureAwait(false);
            if (newProjectDto == null) return BadRequest("Update failed");
            return Ok(pressReleaseDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService._media.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetMenus([FromRoute] int Id)
        {
            if (Id == 0) return BadRequest("Input not valid or null");
            var category = await _dataService._media.GetMenus(Id).ConfigureAwait(false);
            if (category == null) return NotFound("headings not found");
            return Ok(category);
        }

        [AllowAnonymous]
        [HttpGet("{Heading}")]
        public async Task<IActionResult> GetNewsData([FromRoute] int Heading)
        {
            var ContactUsResult = await _dataService._media.GetNewsData(Heading).ConfigureAwait(false);
            if (ContactUsResult == null) return NotFound("Data  not found");
            return Ok(ContactUsResult);
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> GetNewsYear()
        //{
        //    var ContactUs = await _dataService._media.GetNewsYear().ConfigureAwait(false);
        //    if (ContactUs == null || ContactUs.Count <= 0) return NotFound("News Data not found");
        //    return Ok(ContactUs);
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getlastrecord()
        {
            var ContactUs = await _dataService._media.Getlastrecord().ConfigureAwait(false);
            if (ContactUs == null) return NotFound("Data not found");
            return Ok(ContactUs);
        }
    }
}