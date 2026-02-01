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
    public class MenuHeadingsController : Controller
    {
        private readonly IDataService _dataService;

        public MenuHeadingsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // POST: Categories/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService.MenuHeadings.Add(modelDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // GET Categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var categories = await _dataService.MenuHeadings.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Menu Headings not found");
            return Ok(categories);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetForMenu()
        {
            var categories = await _dataService.MenuHeadings.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Menu Headings not found");
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{Heading}")]
        public async Task<IActionResult> GetMenus([FromRoute] string Heading)
        {
            if (Heading == null) return BadRequest("Input not valid or null");
            var category = await _dataService.MenuHeadings.Get(Heading).ConfigureAwait(false);
            //if (category == null) return NotFound("Menu headings not found");
            return Ok(category);
        }

        [AllowAnonymous]
        // GET Categories/GetCategoriesWithAll
        [HttpGet]
        public async Task<IActionResult> GetMenuHeadingsWithAll()
        {
            var Categories = await _dataService.MenuHeadings.GetCategoriesWithAll().ConfigureAwait(false);
            if (Categories == null || Categories.Count <= 0) return NotFound("Menu Headings not found");
            return Ok(Categories);
        }
        // GET Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService.MenuHeadings.Get(id).ConfigureAwait(false);
            if (category == null) return NotFound("Menu headings not found");
            return Ok(category);
        }
        // PUT: Categories/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] MenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService.MenuHeadings.Update(modelDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Update failed");
            return Ok(categoryDto);
        }
        // GET Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTest([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService.MenuHeadings.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (category == null) return NotFound("Menu headings not found");
            return Ok(category);
        }

        // DELETE: Categories/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.MenuHeadings.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMenuContent([FromRoute] int id, [FromBody] MenuHeadingsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var Result = await _dataService.MenuHeadings.EditMenuContent(modelDto).ConfigureAwait(false);
            if (Result == null) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePriority([FromBody] List<MenuHeadingsListPriorityDto> lstMenuDTOs)
        {
            int Result = await _dataService.MenuHeadings.UpdateMenus(lstMenuDTOs).ConfigureAwait(false);
            if (Result == 0) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }

        [AllowAnonymous]
        [HttpGet("{q}")]
        public async Task<IActionResult> GetSearch([FromRoute] string q)
        {
            if (q == null) return BadRequest("Input not valid or null");
            var category = await _dataService.MenuHeadings.GetSearch(q).ConfigureAwait(false);
            if (category == null) return NotFound("Menu headings not found");
            return Ok(category);
        }

    }
}