using Application.Dtos;
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
    public class VendorDetailsModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        private readonly IFileService _fileService;
        public VendorDetailsModel(IHttpClientService httpClient, INotyfService notyf, IFileService fileService)
        {
            _httpClient = httpClient;
            _notyf = notyf;
            _fileService = fileService;
        }

        [BindProperty] public VendorGSTDataDTO vendordata { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var Ids1 = HttpContext.Session.GetObjectFromSession<Dictionary<string, object>>("gstdata");
            if (Ids1 == null)
            {
                return RedirectToPage("/Vendor/UpdateVendorGST");
            }

            var modelResponse = await _httpClient.GetAsync("Vendor/GetVenderGSTDetails", false, Ids1["pkid"].ToString()).ConfigureAwait(false);

            vendordata = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<VendorGSTDataDTO>(modelResponse) : null;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (vendordata.GST_Number == "")
            {
                _notyf.Error("Please enter GST Number");
            }

            if (vendordata.GST_File != null)
            {
                //...........Check Valid File ...................
                if (!_fileService.CheckValidFile(vendordata.GST_File))
                {
                    //......Redirect  
                    _notyf.Information("Please Upload Valid File");
                    //return RedirectToPage("Index");
                }
                vendordata.GST_FileName = await _fileService.SaveImageAsync(@"\app\writereaddata\VenderGST\", vendordata.GST_File);
                vendordata.GST_File = null;
            }

            var postdto = new VendorGSTPostDTO();
            postdto.Id = vendordata.Id;
            postdto.GST_Number = vendordata.GST_Number;
            postdto.GST_FileName = vendordata.GST_FileName;
            postdto.IP = HttpContext.Connection.RemoteIpAddress.ToString();


            var modelResponse = await _httpClient.PutAsync("Vendor/UpdateVenderInfo", false, postdto.Id, postdto).ConfigureAwait(false);

            var res = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<string>(modelResponse) : null;

            if (res == "Success")
            {
                _notyf.Success("Information Added Successfully");
            }

            return Page();
        }
    }
}
