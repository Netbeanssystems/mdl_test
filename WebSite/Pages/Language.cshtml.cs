//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace WebSite
//{
//    public class LanguageModel : PageModel
//    {
//        public IActionResult OnGetReturn(string ReturnUrl)
//        {
//            var lang = HttpContext.Session.GetString("Lang");

//            //English/investor
//            if (!string.IsNullOrEmpty(lang))
//            {
//                switch (lang)
//                {
//                    case "English":
//                        {
//                            HttpContext.Session.SetString("Lang", "Hindi");
//                            ReturnUrl = ReturnUrl.Replace("/English", "/Hindi");
//                            break;
//                        }
//                    case "Hindi":
//                        {
//                            HttpContext.Session.SetString("Lang", "English");
//                            ReturnUrl = ReturnUrl.Replace("/Hindi", "/English");
//                            break;
//                        }
//                    default:
//                        {
//                            HttpContext.Session.SetString("Lang", "English");
//                            ReturnUrl = ReturnUrl.Replace("/Hindi", "/English");
//                            break;
//                        }
//                }
//            }
//            else
//            {
//                HttpContext.Session.SetString("Lang", "English");
//            }
//            return LocalRedirect(ReturnUrl);
//            //return Redirect(ReturnUrl);
//        }
//    }
//}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebSite
{
    public class LanguageModel : PageModel
    {
        // Direct access to /language returns a 404 error
        public IActionResult OnGet()
        {
            return NotFound();
        }

        // Toggles language and redirects back
        public IActionResult OnGetReturn(string? returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return NotFound();
            }

            var currentLang = HttpContext.Session.GetString("Lang");

            if (currentLang == "English")
            {
                HttpContext.Session.SetString("Lang", "Hindi");
                returnUrl = returnUrl.Replace("/English", "/Hindi", System.StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // Default fallback: switch to English if current lang is "Hindi", null, or anything else
                HttpContext.Session.SetString("Lang", "English");
                returnUrl = returnUrl.Replace("/Hindi", "/English", System.StringComparison.OrdinalIgnoreCase);
            }

            return LocalRedirect(returnUrl);
        }
    }
}
