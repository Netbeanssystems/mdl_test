using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class NewsController : Controller
    {
        private readonly IDataService _dataService;
        public NewsController(IDataService dataService)
        {
            _dataService = dataService;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewsDTO NewsDto)
        {
            if (NewsDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService.News.Add(NewsDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Create failed");
            return Ok(NewsDto);
        }

        //--------------------------Old 
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _dataService.News.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("News not found");
            return Ok(categories);
        }

        //------------------Karn 11Dec 2023 ----------
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllNewsList()
        {
            var categories = await _dataService.News.GetAllNewsList().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("News not found");
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> MostViewed()
        {
            var categories = await _dataService.News.MostViewed().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Most Viewed not found");
            return Ok(categories);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> WhatsNew()
        {
            var categories = await _dataService.News.WhatsNew().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("What's New not found");
            return Ok(categories);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> MinistryIndustryUpdates()
        {
            var categories = await _dataService.News.MinistryIndustryUpdates().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Ministry Industry Updates not found");
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{Heading}")]
        public async Task<IActionResult> GetMenus([FromRoute] string Heading)
        {
            if (Heading == null) return BadRequest("Input not valid or null");
            var category = await _dataService.News.Get(Heading).ConfigureAwait(false);
            if (category == null) return NotFound("News Heading not found");
            return Ok(category);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getorderdesc([FromRoute] string Heading)
        {
            var category = await _dataService.News.Getorderdesc().ConfigureAwait(false);
            if (category == null) return NotFound("News Heading not found");
            return Ok(category);
        }

        [AllowAnonymous]
        // GET Categories/GetCategoriesWithAll
        [HttpGet]
        public async Task<IActionResult> GetMenuHeadingsWithAll()
        {
            var Categories = await _dataService.News.GetCategoriesWithAll().ConfigureAwait(false);
            if (Categories == null || Categories.Count <= 0) return NotFound("News Headings not found");
            return Ok(Categories);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var newProject = await _dataService.News.Get(id).ConfigureAwait(false);
            if (newProject == null) return NotFound("New Projects not found");
            return Ok(newProject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] NewsDTO NewsDto)
        {
            if (NewsDto == null) return BadRequest("Input not valid or null");
            if (id != NewsDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newProjectDto = await _dataService.News.Update(NewsDto).ConfigureAwait(false);
            if (newProjectDto == null) return BadRequest("Update failed");
            return Ok(NewsDto);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTest([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService.News.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (category == null) return NotFound("News headings not found");
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.News.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditMenuContent([FromRoute] int id, [FromBody] NewsDTO NewsDto)
        {
            if (NewsDto == null) return BadRequest("Input not valid or null");
            if (id != NewsDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var Result = await _dataService.News.EditMenuContent(NewsDto).ConfigureAwait(false);
            if (Result == null) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePriority([FromBody] List<NewsListPriorityDto> lstMenuDTOs)
        {
            int Result = await _dataService.News.UpdateMenus(lstMenuDTOs).ConfigureAwait(false);
            if (Result == 0) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }
    }
}
