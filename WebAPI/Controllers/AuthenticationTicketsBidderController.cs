using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationTicketsBidderController : Controller
    {
        private readonly IDataService _dataService;
        public AuthenticationTicketsBidderController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET AuthenticationTicketsBidder/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.AuthenticationTicketsBidder.Get(id).ConfigureAwait(false);
            //if (modelDto == null) return NotFound("Authentication Ticket not found");
            return Ok(modelDto);
        }
        // POST: AuthenticationTicketsBidder/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuthenticationTicketsBidderDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.AuthenticationTicketsBidder.Create(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }
        // PUT: AuthenticationTicketsBidder/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] AuthenticationTicketsBidderDTO inputModel)
        {
            if (string.IsNullOrEmpty(id) || inputModel == null || id != inputModel.Id) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.AuthenticationTicketsBidder.Update(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Update failed");
        }
        // DELETE: AuthenticationTicketsBidder/Delete/5/false
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Input not valid or null");
            var rowsChanged = await _dataService.AuthenticationTicketsBidder.Delete(id).ConfigureAwait(false);
            if (rowsChanged > 0) return Ok(rowsChanged);
            return BadRequest("Delete failed. There might be active child records.");
        }
        // POST: AuthenticationTicketsBidder/Upsert
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] AuthenticationTicketsBidderDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null");
            var modelDtoTR = await _dataService.AuthenticationTicketsBidder.Upsert(modelDto).ConfigureAwait(false);
            if (modelDtoTR != null) return Ok(modelDtoTR);
            return BadRequest("Upsertion failed");
        }
    }
}
