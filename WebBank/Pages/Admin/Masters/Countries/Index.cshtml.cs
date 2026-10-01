using Application.Dtos;
using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBank.Extensions;

namespace WebBank.Pages.Admin.Masters.Countries
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class IndexModel : PageModel
    {
        Validate objbal = new Validate();
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public IndexModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public List<CountriesVM> ModelVms { get; set; }
        [BindProperty] public CountriesDTO ModelDto { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("Countries/Get", true).ConfigureAwait(false);
            if (modelResponse == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelVms = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<CountriesVM>>(modelResponse) : null;
            if (ModelVms == null || ModelVms.Count <= 0)
                _notyf.Error("Record not found");
            return Page();
        }
        public async Task<JsonResult> OnGetModel(int id)
        {
            var response = await _httpClient.GetAsync("Countries/Get", true, id).ConfigureAwait(false);
            ModelDto = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<CountriesDTO>(response) : null;
            return new JsonResult(ModelDto);
        }
        public async Task<IActionResult> OnPostManage()
        {
            var filteredmodel = new CountriesDTO();
            filteredmodel.Id = ModelDto.Id;
            filteredmodel.Name = objbal.OnlyValid(ModelDto.Name);
            filteredmodel.Code3 = objbal.OnlyValid(ModelDto.Code3);
            filteredmodel.Code2 = objbal.OnlyValid(ModelDto.Code2);
            filteredmodel.Capital = objbal.OnlyValid(ModelDto.Capital);
            filteredmodel.CurrencyCode = objbal.OnlyValid(ModelDto.CurrencyCode);

            ModelDto = ModelAuditor<CountriesDTO>.SetAudit(User.Identity.Name, ModelDto.Id == 0 ? "Create" : "Edit", HttpContext.Connection.RemoteIpAddress.ToString(), filteredmodel);
            if (!ModelState.IsValid)
            {
                _notyf.Error($"{ModelState.GetErrorMessageString()}");
                return RedirectToPage("Index");
            }
            var response = ModelDto.Id == 0
            ? await _httpClient.PostAsync("Countries/Create", true, filteredmodel).ConfigureAwait(false)
            : await _httpClient.PutAsync("Countries/Edit", true, filteredmodel.Id, filteredmodel)
            .ConfigureAwait(false);
            if (response == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            ModelDto = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<CountriesDTO>(response) : null;
            if (ModelDto == null)
                _notyf.Error("Save failed");
            else
                _notyf.Success("Saved successfully");
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnGetDelete(int id)
        {
            var DeleteResult = await _httpClient.DeleteAsync("Countries/Delete", true, id).ConfigureAwait(false);
            if (DeleteResult == "unauthorized") return new JsonResult("unauthorized");
            var RowsChanged = !string.IsNullOrEmpty(DeleteResult) && Convert.ToInt32(DeleteResult) > 0;
            return RowsChanged ? new JsonResult("success") : new JsonResult("fail");
        }
    }
}