using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace WebAPI.Controllers
{
    [Authorize(Roles = "BidderSuperAdmin,CommercialExecutive")]
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IDataService _dataService;
        public ProjectsController(IDataService dataService)
        {
            _dataService = dataService;
        }
        // GET
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
            {
            var modelVms = await _dataService.Projects.Get().ConfigureAwait(false);
            if (modelVms == null || modelVms.Count <= 0) return NotFound("States not found");
            return Ok(modelVms);
        }
        // GET:Id
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.Projects.Get(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Project not found");
            return Ok(modelDto);
        }

        [HttpGet("GetMultiple/{ids}")]
        public async Task<IActionResult> GetMultiple([FromRoute] string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("Input not valid or null");

            // Split comma-separated IDs and validate
            var idList = ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x.Trim(), out var n) ? n : 0)
                .Where(n => n > 0)
                .ToList();

            if (idList == null || idList.Count == 0)
                return BadRequest("Invalid or empty ID list");

            // Call your data service method that supports multiple IDs
            var modelDtos = await _dataService.Projects.GetByIds(idList).ConfigureAwait(false);

            if (modelDtos == null || !modelDtos.Any())
                return NotFound("No projects found for given IDs");

            return Ok(modelDtos);
        }

        // GET:Id
        [HttpGet("GetYard/{id}")]
        public async Task<IActionResult> GetYard([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Input not valid or null");
            var modelDto = await _dataService.Projects.GetYard(id).ConfigureAwait(false);
            if (modelDto == null) return NotFound("Yard not found");
            return Ok(modelDto);
        }

        [HttpGet("GetYardsByProjectIds/{projectIds}")]
        public async Task<IActionResult> GetYardsByProjectIds([FromRoute] string projectIds)
        {
            if (string.IsNullOrWhiteSpace(projectIds))
                return BadRequest("Project IDs cannot be empty.");

            // Pass all IDs as comma-separated string to service
            var yards = await _dataService.Projects.GetYardsByMultipleProjects(projectIds).ConfigureAwait(false);

            if (yards == null || !yards.Any())
                return NotFound("No yards found for the given project IDs.");

            return Ok(yards);
        }

        [HttpGet("GetProjects")]
        public async Task<IActionResult> GetProjects()
        {
            var modelDto = await _dataService.Projects.GetProjects().ConfigureAwait(false);
            if (modelDto == null) return NotFound("Project not found");
            return Ok(modelDto);
        }
        // POST: Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] BidderProjectsDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

            if (inputModel.Id == 0)
            {
                inputModel.CreatedDate = DateTime.Now;
                inputModel.CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value;
            }
            else
            {
                inputModel.ModifiedDate = DateTime.Now;
                inputModel.ModifiedBy = User.FindFirst(ClaimTypes.Name)?.Value;
            }
            inputModel.IP = HttpContext.Connection.RemoteIpAddress?.ToString();

            var modelDto = await _dataService.Projects.Create(inputModel).ConfigureAwait(false);
            if (modelDto != null) return Ok(modelDto);
            return BadRequest("Create failed");
        }
        // POST: Create
        [HttpPost("UploadQuota")]
        public async Task<IActionResult> UploadQuota([FromBody] UpdateQuotaDTO inputModel)
        {
            if (inputModel == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());
            //inputModel = ModelAuditor<UpdateQuotaDTO>.SetAudit(User.Identity?.Name, inputModel.Id == 0 ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress?.ToString(), inputModel);
            var rowAffects = await _dataService.Projects.UploadQuota(inputModel).ConfigureAwait(false);
            if (rowAffects > 0) return Ok(inputModel);
            return BadRequest("Create failed");
        }
    }
}
