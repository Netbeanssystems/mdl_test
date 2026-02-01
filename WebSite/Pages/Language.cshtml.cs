using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebSite
{
    public class LanguageModel : PageModel
    {
        public IActionResult OnGetReturn(string ReturnUrl)
        {
            var lang = HttpContext.Session.GetString("Lang");

            //English/investor
            if (!string.IsNullOrEmpty(lang))
            {
                switch (lang)
                {
                    case "English":
                        {
                            HttpContext.Session.SetString("Lang", "Hindi");
                            ReturnUrl = ReturnUrl.Replace("/English", "/Hindi");
                            break;
                        }
                    case "Hindi":
                        {
                            HttpContext.Session.SetString("Lang", "English");
                            ReturnUrl = ReturnUrl.Replace("/Hindi", "/English");
                            break;
                        }
                    default:
                        {
                            HttpContext.Session.SetString("Lang", "English");
                            ReturnUrl = ReturnUrl.Replace("/Hindi", "/English");
                            break;
                        }
                }
            }
            else
            {
                HttpContext.Session.SetString("Lang", "English");
            }
            return LocalRedirect(ReturnUrl);
            //return Redirect(ReturnUrl);
        }
    }
}
