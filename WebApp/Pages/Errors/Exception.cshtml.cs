using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace WebApp.Pages.Errors
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ExceptionModel : PageModel
    {
        public IActionResult OnGet(int? statusCode = null)
        {
            switch (statusCode)
            {
                case 404:
                    return RedirectToPage("/Errors/PageNotFound");
                case 500:
                    return RedirectToPage("/Errors/InternalServerError");
                default:
                    return Page();
            }
        }
    }
}
