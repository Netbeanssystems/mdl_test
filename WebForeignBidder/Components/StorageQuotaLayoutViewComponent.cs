using Application.ServiceInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebForeignBidder.Components
{
    public class StorageQuotaLayoutViewComponent : ViewComponent
    {
        private readonly IHttpClientService _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StorageQuotaLayoutViewComponent(IHttpClientService httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userProjectIds = _httpContextAccessor.HttpContext?.User?.FindFirst("proj")?.Value;
            var projectsResult = "";

            //if (string.IsNullOrEmpty(userProjectIds))
            //    return View(new List<ProjectResponse>());

            if (User.IsInRole("CommercialExecutive"))
            {
                projectsResult = await _httpClient.GetAsync("Projects/Get", true).ConfigureAwait(false);
                var projects = JsonConvert.DeserializeObject<List<ProjectResponse>>(projectsResult) ?? new List<ProjectResponse>();
                var userProjectIdsList = userProjectIds.Split(',').Select(id => int.Parse(id.Trim())).ToList();
                var filteredProjects = projects.Where(p => userProjectIdsList.Contains(p.Id)).ToList();

                return View(filteredProjects);
            }
            else if (User.IsInRole("GeneralUpload")) {
                //projectsResult = await _httpClient.GetAsync("BidderGeneralDoc/Get", true).ConfigureAwait(false);
                //return View(projectsResult);
                var projectsResult1 = await _httpClient.GetAsync("BidderGeneralDoc/Get", true).ConfigureAwait(false);
                var project1List = JsonConvert.DeserializeObject<List<Project1Response>>(projectsResult1) ?? new List<Project1Response>();


                // Map to ProjectResponse for the View
                var projects = project1List.Select(p => new ProjectResponse
                {
                    Id = p.Id,
                    OccupiedQuota = p.OccupiedQuota,
                    TotalQuota = p.TotalQuota,
                    ProjectName = "General Uploads",  // since not available
                    Yard = string.Empty,
                    Remarks = string.Empty
                }).ToList();

                return View(projects);
            
            }
            else
            {
                if (userProjectIds == "")
                {
                    return View(new List<ProjectResponse>());
                }
            }
            if (projectsResult == "unauthorized")
            {
                return View(new List<ProjectResponse>());
            }
            
            //if (userProjectIds != "") 
            //{
            //    var projects = JsonConvert.DeserializeObject<List<ProjectResponse>>(projectsResult) ?? new List<ProjectResponse>();
            //    var userProjectIdsList = userProjectIds.Split(',').Select(id => int.Parse(id.Trim())).ToList();
            //    var filteredProjects = projects.Where(p => userProjectIdsList.Contains(p.Id)).ToList();

            //    return View(filteredProjects);
            //}
            return View();
            }
    }

}
