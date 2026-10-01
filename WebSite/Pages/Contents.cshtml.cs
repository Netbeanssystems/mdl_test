using Application.ServiceInterfaces;
using Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;
namespace WebSite.Pages
{
    public class ContentsModel : PageModel
    {
        private readonly IHttpClientService _httpClient;

        public ContentsModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        [FromRoute] public string Heading { get; set; }
        [BindProperty] public CommonVM ModelVM { get; set; }
        public MenuHeadingsVM Menus { get; set; }

        public async Task<IActionResult> OnGet(int? News)
        {
            if (News == null)
            {
                var Result = await _httpClient.GetAsync("MenuHeadings/GetMenus", false, Heading.Replace("-", " ").ToLower());
                if (Result != null)
                {

                    Menus = !string.IsNullOrEmpty(Result) ? JsonConvert.DeserializeObject<MenuHeadingsVM>(Result) : null;
                    if (HttpContext.Session.GetString("Lang") == null || HttpContext.Session.GetString("Lang") == "English")
                    {
                        ModelVM = new CommonVM
                        {
                            Heading = Menus.EnglishHeadingName,
                            HindiHeadingName = Menus.HindiHeadingName,
                            Link = Menus.EnglishAttachment,
                            Content = Menus.EnglishContentDesc,
                            Title = Menus.Title,
                            Description = Menus.Description,
                            Keywords = Menus.Keyword,
                            KeywordsHindi = Menus.KeywordHindi,
                            DescriptionHindi = Menus.DescriptionHindi,
                            UpdateDate = Menus.UpdateDate
                        };
                        HttpContext.Session.SetString("LastDate", Menus.UpdateDate.ToString());
                    }
                    else
                    {
                        ModelVM = new CommonVM
                        {
                            Heading = Menus.HindiHeadingName,
                            Link = Menus.HindiAttachment,
                            Content = Menus.HindiContentDesc,
                            HindiTitle = Menus.HindiTitle,
                            Description = Menus.Description,
                            Keywords = Menus.Keyword,
                            KeywordsHindi = Menus.KeywordHindi,
                            DescriptionHindi = Menus.DescriptionHindi,
                            UpdateDate = Menus.UpdateDate
                        };
                        HttpContext.Session.SetString("LastDate", Menus.UpdateDate.ToString());
                    }
                }
                else
                {
                    return RedirectToPage("Index");
                }
            }
            else
            {
                var NewsResult = await _httpClient.GetAsync("News/GetMenus", false, Heading.Replace("-", " ").ToLower());
                var NewsMenus = !string.IsNullOrEmpty(NewsResult) ? JsonConvert.DeserializeObject<NewsVM>(NewsResult) : null;
                if (HttpContext.Session.GetString("Lang") == null || HttpContext.Session.GetString("Lang") == "English")
                {
                    ModelVM = new CommonVM
                    {
                        Content = NewsMenus.EnglishContentDesc,
                        UpdateDate = Menus.UpdateDate
                    };
                }
                else
                {
                    ModelVM = new CommonVM
                    {
                        Content = NewsMenus.HindiContentDesc,
                        UpdateDate = Menus.UpdateDate
                    };
                }

            }
            TempData["CategoryTabName"] = ModelVM.UpdateDate ?? DateTime.ParseExact("10-10-2021", "dd-MM-yyyy", CultureInfo.InvariantCulture);
            return Page();
        }


    }
}
