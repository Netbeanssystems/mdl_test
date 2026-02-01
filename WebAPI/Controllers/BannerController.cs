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
    public class BannerController : Controller
    {
        private readonly IDataService _dataService;

        public BannerController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BannerDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService._banner.Add(modelDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }

        //------------------------Old Code Karn-------------
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var categories = await _dataService._banner.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Home Page Banners Menu not found");
            return Ok(categories);
        }

        //-----------------------------------New Karn Code 8 Dec 2023---------------
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBanner()
        {
            var categories = await _dataService._banner.GetAllBanner().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Home Page Banners Menu not found");
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMenuHeadingsWithAll()
        {
            var Categories = await _dataService._banner.GetCategoriesWithAll().ConfigureAwait(false);
            if (Categories == null || Categories.Count <= 0) return NotFound("Home Page Banners Menu not found");
            return Ok(Categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService._banner.Get(id).ConfigureAwait(false);
            if (category == null) return NotFound("Home Page Banner Menu not found");
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] BannerDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService._banner.Update(modelDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Update failed");
            return Ok(categoryDto);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetTest([FromRoute] int id)
        //{
        //    if (id <= 0) return BadRequest("Input not valid or null");
        //    var category = await _dataService._banner.GetCategoriesWithAllTest(id).ConfigureAwait(false);
        //    if (category == null) return NotFound("Home Page Banner Menu not found");
        //    return Ok(category);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService._banner.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMenuContent([FromRoute] int id, [FromBody] BannerDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var Result = await _dataService._banner.EditMenuContent(modelDto).ConfigureAwait(false);
            if (Result == null) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePriority([FromBody] List<MenuHeadingListPriorityDto> lstMenuDTOs)
        {
            int Result = await _dataService._banner.UpdateMenus(lstMenuDTOs).ConfigureAwait(false);
            if (Result == 0) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }
    }
}