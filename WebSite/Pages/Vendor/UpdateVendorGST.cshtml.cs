using Application.Dtos;
using Application.Helpers;
using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Extensions;

namespace WebSite.Pages.Vendor
{
    public class UpdateVendorGSTModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public UpdateVendorGSTModel(IHttpClientService httpClient, INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }

        [BindProperty] public VendorGSTDTO vendor { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!Captcha.ValidateCaptchaCode(vendor.CaptchaCode, HttpContext))
            {
                ModelState.AddModelError("Captcha", "Please enter correct captcha");
            }


            var modelResponse = await _httpClient.GetAsync("Vendor/GetVenderGSTInfo", false, vendor.VendorCode, vendor.PANNo).ConfigureAwait(false);

            int res = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<int>(modelResponse) : 0;

            if (res == 0)
            {
                _notyf.Error("Invalid Details");
                return Page();
            }
            else
            {
                var gstvalue = new Dictionary<string, object> { { "pkid", res } };
                HttpContext.Session.SetObjectToSession("gstdata", gstvalue);
                return RedirectToPage("/Vendor/VendorDetails");
            }
        }
    }
}
