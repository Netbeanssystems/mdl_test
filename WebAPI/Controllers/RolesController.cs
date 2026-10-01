using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class RolesController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        public RolesController(
            IIdentityService identityService,
            IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }
        // GET Roles
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var roles = await _identityService.GetRoles().ConfigureAwait(false);
            if (roles == null || roles.Count <= 0) return NotFound("Roles not found");
            var roleDtos = _mapper.Map<List<RoleDTO>>(roles);
            return Ok(roleDtos);
        }
        // GET Roles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Input not valid or null");
            var role = await _identityService.GetRole(id).ConfigureAwait(false);
            if (role == null) return NotFound("Role not found");
            var roleDto = _mapper.Map<RoleDTO>(role);
            return Ok(roleDto);
        }
        // POST: Roles/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDTO role)
        {
            if (role == null) return BadRequest("Input not valid or null");
            role.Id = Guid.NewGuid().ToString();
            role.Name = string.Concat(role.Name.Where(x => !char.IsWhiteSpace(x)));
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            if (await _identityService.RoleExists(role.Name).ConfigureAwait(false))
                return BadRequest("Role already exists");
            var applicationRole = _mapper.Map<ApplicationRole>(role);
            var result = await _identityService.AddRole(applicationRole).ConfigureAwait(false);
            if (result.Succeeded) return Ok(role);
            return BadRequest("Role couldn't be created");
        }
        // PUT: Roles/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] RoleDTO role)
        {
            if (role == null) return BadRequest("Input not valid or null");
            if (id != role.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            var existingrole = await _identityService.GetRole(id).ConfigureAwait(false);
            if (existingrole == null) return BadRequest("Input not valid or null");
            existingrole.Name = role.Name;
            existingrole.CanView = role.CanView;
            existingrole.CanCreate = role.CanCreate;
            existingrole.CanEdit = role.CanEdit;
            existingrole.CanDelete = role.CanDelete;
            var result = await _identityService.EditRole(existingrole).ConfigureAwait(false);
            if (result.Succeeded) return Ok(existingrole);
            return BadRequest("Role couldn't be updated");
        }
        // DELETE: Roles/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Input not valid or null");
            var role = await _identityService.GetRole(id).ConfigureAwait(false);
            if (role == null) return BadRequest("Role not found");
            var result = await _identityService.DeleteRole(role).ConfigureAwait(false);
            if (result.Succeeded) return Ok(true);
            return BadRequest("Role couldn't be deleted");
        }
        // POST: Roles/UpdatePermissions
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdatePermissions([FromBody] List<RoleDTO> argModelDtos)
        {
            if (argModelDtos == null || argModelDtos.Count <= 0) return BadRequest("Input not valid or null");
            var rowsSucceded = 0;
            foreach (var role in argModelDtos)
            {
                var existingrole = await _identityService.GetRole(role.Id).ConfigureAwait(false);
                existingrole.Name = role.Name;
                existingrole.CanView = role.CanView;
                existingrole.CanCreate = role.CanCreate;
                existingrole.CanEdit = role.CanEdit;
                existingrole.CanDelete = role.CanDelete;
                var result = await _identityService.EditRole(existingrole).ConfigureAwait(false);
                if (result.Succeeded)
                    rowsSucceded += 1;
            }
            return Ok(rowsSucceded);
        }
    }
}
