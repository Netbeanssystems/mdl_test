using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationTicketsController : Controller
    {
        private readonly IDataService _dataService;
        public AuthenticationTicketsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET AuthenticationTickets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.AuthenticationTickets.Get(id).ConfigureAwait(false);
            //if (modelDto == null) return NotFound("Authentication Ticket not found");
            return Ok(modelDto);
        }
        // POST: AuthenticationTickets/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuthenticationTicketsDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.AuthenticationTickets.Create(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }
        // PUT: AuthenticationTickets/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] AuthenticationTicketsDTO inputModel)
        {
            if (string.IsNullOrEmpty(id) || inputModel == null || id != inputModel.Id) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.AuthenticationTickets.Update(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Update failed");
        }
        // DELETE: AuthenticationTickets/Delete/5/false
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.AuthenticationTickets.Delete(id).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // POST: AuthenticationTickets/Upsert
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] AuthenticationTicketsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            var modelDtoTR = await _dataService.AuthenticationTickets.Upsert(modelDto).ConfigureAwait(false);
            if (modelDtoTR != null) return Ok(modelDtoTR);
            return BadRequest("Upsertion failed");
        }
    }
}
