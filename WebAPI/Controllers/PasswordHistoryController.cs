using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class PasswordHistoryController : Controller
    {
        private readonly IDataService _dataService;
        public PasswordHistoryController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET username
        [HttpGet("{username}")]
        public async Task<IActionResult> GetByUsername([FromRoute] string username)
        {
            if (username == null) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.passhistory.GetByUsername(username).ConfigureAwait(false);
            if (modelDto == null) return NotFound("History not found");
            return Ok(modelDto);
        }

        // GET username
        [HttpGet("{username}/{pwd}")]
        public async Task<IActionResult> GetByUsernamePwd([FromRoute] string username, string pwd)
        {
            if (username == null) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.passhistory.GetByUsernamePwd(username, pwd).ConfigureAwait(false);
            if (modelDto == null) return NotFound("History not found");
            return Ok(modelDto);
        }
    }
}
