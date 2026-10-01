using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    //[ApiController]
    [Route("[controller]/[action]")]
    public class ForgetPasswordDetailsController : Controller
    {
        private readonly IDataService _dataService;
        public ForgetPasswordDetailsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET ForgetPasswordDetails
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var modelVMS = await _dataService.ForgetPasswordDetailsServices.Get().ConfigureAwait(false);
            if (modelVMS == null || modelVMS.Count <= 0) return NotFound("Category Not Found!");
            return Ok(modelVMS);
        }
        // GET ForgetPasswordDetails/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            if (id == null) return BadRequest("Input not valid or null.");
            var modelDto = await _dataService.ForgetPasswordDetailsServices.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Category Not Found!");
            return Ok(modelDto);
        }
        // POST: ForgetPasswordDetails/Create
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ForgetPasswordDetailsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ModelDto = await _dataService.ForgetPasswordDetailsServices.Create(modelDto).ConfigureAwait(false);
            if (ModelDto == null) return BadRequest("Create Failed.");
            return Ok(ModelDto);
        }

        // PUT: ForgetPasswordDetails/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] ForgetPasswordDetailsDTO modelDto)
        {
            if (modelDto == null) return BadRequest("Input not valid or null.");
            //if (id != modelDto.Id) return BadRequest("Invalid Id.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var ModelDTO = await _dataService.ForgetPasswordDetailsServices.Update(modelDto).ConfigureAwait(false);
            if (ModelDTO == null) return BadRequest("Update Failed.");
            return Ok(ModelDTO);
        }
        // DELETE: ForgetPasswordDetails/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (id == null) return BadRequest("Input not valid or null.");
            var rowsAffected = await _dataService.ForgetPasswordDetailsServices.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete Failed.");
        }

        [AllowAnonymous]
        [HttpGet("{username}/{date}")]
        //[HttpGet]
        public async Task<IActionResult> GetPasswordsendCount([FromRoute] string username, string date)
        {
            var count = await _dataService.ForgetPasswordDetailsServices.CheckUserDetails(username, date).ConfigureAwait(false);
            if (count == null) return BadRequest("Data not found.");
            return Ok(count);
        }

        [AllowAnonymous]
        [HttpGet("{username}/{date}")]
        //[HttpGet]
        public async Task<IActionResult> GetHitCount([FromRoute] string username, string date)
        {
            var currentdate = DateTime.Now.Date;
            var count = await _dataService.ForgetPasswordDetailsServices.CheckUserDetails(username, currentdate.ToString()).ConfigureAwait(false);
            if (count == null) return BadRequest("Data not found.");
            return Ok(count);
        }

    }
}
