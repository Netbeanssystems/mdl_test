using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RoleMenusController : Controller
    {
        private readonly IDataService _dataService;
        public RoleMenusController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET RoleMenus
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var modelVms = await _dataService.RoleMenus.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("Role menus not found");
            return Ok(modelVms);
        }
        // GET RoleMenus/5
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.RoleMenus.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Menus not found");
            return Ok(modelDto);
        }
        // POST: RoleMenus/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] RoleMenusDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.RoleMenus.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Create failed");
            return Ok(argModelDto);
        }
        // PUT: RoleMenus/Edit/5
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] RoleMenusDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (id != argModelDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelDto = await _dataService.RoleMenus.Update(argModelDto).ConfigureAwait(false);
            if (modelDto == null) return BadRequest("Update failed");
            return Ok(modelDto);
        }
        // DELETE: RoleMenus/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.RoleMenus.Delete(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed. There might be active child records.");
        }
        //// GET RoleMenus/GetMenusByRole
        //[HttpGet("GetMenusByRole/{rolename}")]
        //public async Task<IActionResult> GetMenusByRole([FromRoute] string rolename)
        //{
        //    var roleMenuVm = await _dataService.RoleMenus.GetAllByRole(rolename).ConfigureAwait(false);
        //    if (roleMenuVm == null) return NotFound("Role Menus not found");
        //    return Ok(roleMenuVm);
        //}
        // GET RoleMenus/GetAllByRole
        [HttpGet("GetAllByRole/{rolename}")]
        public async Task<IActionResult> GetAllByRole([FromRoute] string rolename)
        {
            var modelVms = await _dataService.RoleMenus.GetAllByRole(rolename).ConfigureAwait(false);
            if (modelVms == null) return NotFound("Role Menus not found");
            return Ok(modelVms);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        // POST: RoleMenus/AddToRole
        [HttpPost("AssignToRole")]
        public async Task<IActionResult> AssignToRole([FromBody] RoleMenusVM argModelVm)
        {
            if (argModelVm == null) return BadRequest("Input not valid or null");
            var modelDto = new RoleMenusDTO { MenuId = argModelVm.menuId, RoleName = argModelVm.roleName };
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelVm = await _dataService.RoleMenus.AssignToRole(modelDto).ConfigureAwait(false);
            if (modelVm == null) return NotFound("Menus couldn't be assigned");
            return Ok(modelVm);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        // POST: RoleMenus/RemoveFromRole
        [HttpPost("RemoveFromRole")]
        public async Task<IActionResult> RemoveFromRole([FromBody] RoleMenusVM argModelVm)
        {
            if (argModelVm == null) return BadRequest("Input not valid or null");
            var modelDto = new RoleMenusDTO { MenuId = argModelVm.menuId, RoleName = argModelVm.roleName };
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var modelVm = await _dataService.RoleMenus.RemoveFromRole(modelDto).ConfigureAwait(false);
            if (modelVm == null) return NotFound("Menus couldn't be removed");
            return Ok(modelVm);
        }
        [Authorize(Roles = "SuperAdmin,Admin,BidderSuperAdmin")]
        // POST: RoleMenus/UpdateMenus
        [HttpPost("UpdateMenus")]
        public async Task<IActionResult> UpdateMenus([FromBody] List<RoleMenusDTO> argModelDtos)
        {
            if (argModelDtos == null || argModelDtos.Count <= 0) return BadRequest("Input not valid or null");
            var result = await _dataService.RoleMenus.UpdateMenus(argModelDtos).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
