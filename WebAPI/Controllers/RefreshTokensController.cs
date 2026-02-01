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
    public class RefreshTokensController : Controller
    {
        private readonly IDataService _dataService;
        public RefreshTokensController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET RefreshTokens
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var refreshTokens = await _dataService.RefreshTokens.Get().ConfigureAwait(false);
            if (refreshTokens == null || refreshTokens.Count <= 0) return NotFound("RefreshTokens not found");
            return Ok(refreshTokens);
        }
        // GET RefreshTokens/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var refreshToken = await _dataService.RefreshTokens.Get(id).ConfigureAwait(false);
            if (refreshToken == null) return NotFound("RefreshToken not found");
            return Ok(refreshToken);
        }
        // POST: RefreshTokens/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RefreshTokenDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var refreshTokenDto = await _dataService.RefreshTokens.Create(modelDto).ConfigureAwait(false);
            if (refreshTokenDto == null) return BadRequest("Create failed");
            return Ok(modelDto);
        }
        // PUT: RefreshTokens/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] RefreshTokenDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            if (id != modelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var refreshTokenDto = await _dataService.RefreshTokens.Update(modelDto).ConfigureAwait(false);
            if (refreshTokenDto == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: RefreshTokens/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.RefreshTokens.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed. There might be active child records.");
        }
    }
}
