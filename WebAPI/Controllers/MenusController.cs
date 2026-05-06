using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class MenusController : Controller
    {
        private readonly IDataService _dataService;
        public MenusController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET Menus
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.Menus.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Menus not found");
            return Ok(modelVms);
        }
        // GET Menus/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.Menus.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Menus not found");
            return Ok(modelDto);
        }
        // POST: Menus/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Menus.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // PUT: Menus/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] MenuDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (id != argModelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.Menus.Update(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: Menus/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.Menus.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // GET Menus/GetMenusWithAll
        [HttpGet]
        public async Task<IActionResult> GetWithAll()
        {
            var modelVms = await _dataService.Menus.GetWithAll().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Menus not found");
            return Ok(modelVms);
        }
        // GET Menus/GetAllByRole
        [HttpGet("{role}")]
        public async Task<IActionResult> GetAllByRole([FromRoute] string role)
        {
            var modelVms = await _dataService.Menus.GetAllByRole(role).ConfigureAwait(false);
            //if (modelVms == null || modelVms.Count <= 0) return NotFound("Menus not found");
            return Ok(modelVms);
        }
    }
}
