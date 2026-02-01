using Application.Dtos;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Extensions;

namespace WebSite.Pages
{
    public class SearchModel : PageModel
    {
        Validate objbal = new Validate();
        private readonly IHttpClientServiceSite _httpClient;
        public string output { get; set; }
        private readonly INotyfService _notyf;
        public SearchModel(IHttpClientServiceSite httpClient, INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public List<SearchResults> searchVMList = new List<SearchResults>();
        [BindProperty] public CommonVM ModelVM { get; set; }
        [BindProperty] public CommonVM otherlinkModelVM { get; set; }
        public MenuHeadingsVM Menus { get; set; }
        public OtherLinkHeadingVM Menusotherlink { get; set; }

        public async Task<IActionResult> OnGetAsync(string output)
        {
            output = objbal.OnlyValid(output);
            if (output != "" && output != null)
            {
                TempData["Query"] = output;
            }

            var currentdate = DateTime.Now.Date;
            var Username = HttpContext.Connection.RemoteIpAddress.ToString();

            var resultcount = await _httpClient.GetAsync("ForgetPasswordDetails/GetHitCount", false, Username, "dd").ConfigureAwait(false);
            var DetailCount = !string.IsNullOrEmpty(resultcount) ? JsonConvert.DeserializeObject<List<ForgetPasswordDetailsVM>>(resultcount) : null;

            if (resultcount != null)
            {
                if (DetailCount.Count >= 50)
                {
                    _notyf.Error("Search could not be generated more than 3 times.");
                    return LocalRedirect("/Index");
                }
            }

            if (output != "" && output != null)
            {
                //Search for main menu heading...
                    string modelResponse = await _httpClient.GetAsync("MenuHeadings/GetSearch", false, output.Replace("-", " ").Replace("-", "/").ToLower()).ConfigureAwait(false);
                var response = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<List<MenuHeadingsVM>>(modelResponse) : null;
                if (response != null)
                {
                    if (response.Count > 0)
                    {
                        foreach (var list in response)
                        {
                            if (searchVMList.Any(x => x.Particulars == list.EnglishHeadingName))
                                continue;
                            SearchResults sr = new SearchResults();
                            sr.Particulars = list.EnglishHeadingName;
                            sr.Content = list.EnglishPageLink;
                            sr.MenuId = list.Id;
                            sr.MenuParentId = list.ParentId;
                            searchVMList.Add(sr);
                        }
                    }
                }

                string modelResponse1 = await _httpClient.GetAsync("OtherLinkHeading/GetSearchfromotherlinks", false, output.Replace("-", " ").Replace("-", "/").ToLower()).ConfigureAwait(false);
                var response1 = !string.IsNullOrEmpty(modelResponse1) ? JsonConvert.DeserializeObject<List<OtherLinkHeadingVM>>(modelResponse1) : null;
                if (response1 != null)
                {
                    if (response1.Count > 0)
                    {
                        foreach (var list in response1)
                        {
                            SearchResults sr = new SearchResults();
                            sr.Particulars = list.EnglishHeadingName;
                            sr.Content = list.EnglishPageLink;
                            sr.MenuId = list.Id;
                            sr.MenuParentId = list.ParentId;
                            searchVMList.Add(sr);
                        }
                    }
                }
            }

            ForgetPasswordDetailsDTO newt = new ForgetPasswordDetailsDTO();
            newt.UserId = Username;
            newt.Email = Username;
            newt.CreatedDate = System.DateTime.Now.Date;
            //var Result = await _httpClient.PostAsync("PasswordResetLimit/Create", false, PasswordResetLimitDTO);
            var Result = await _httpClient.PostAsync("ForgetPasswordDetails/Create", false, newt);

            return Page();

        }


        public async Task<IActionResult> OnGetContents(string output)
        {
            if (output != "" && output != null)
            {
                //Search for main menu heading...
                var Result = await _httpClient.GetAsync("MenuHeadings/GetMenus", false, output);
                Menus = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<MenuHeadingsVM>(Result) : null;
                if (Menus != null)
                {
                    ModelVM = new CommonVM
                    {
                        Heading = Menus.EnglishHeadingName,
                        Link = Menus.EnglishAttachment,
                        Content = Menus.EnglishContentDesc,
                        Title = Menus.Title
                    };

                }
            }
            if (output != "" && output != null && Menus == null)
            {
                //Search for otherlink heading...
                var Resultotherlink = await _httpClient.GetAsync("OtherLinkHeading/GetMenus", false, output);
                Menusotherlink = !string.IsNullOrEmpty(Resultotherlink) ? JsonConvert.DeserializeObject<OtherLinkHeadingVM>(Resultotherlink) : null;
                if (Menusotherlink != null)
                {
                    otherlinkModelVM = new CommonVM
                    {
                        Heading = Menus.EnglishHeadingName,
                        Link = Menus.EnglishAttachment,
                        Content = Menus.EnglishContentDesc,
                        Title = Menus.Title
                    };
                }
            }
            else
            {

            }
            return new JsonResult(1);
        }
    }
}
