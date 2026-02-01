using Application.ServiceInterfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebSite.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;

        public IndexModel(IHttpClientService httpClient, INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }

        public IActionResult OnGetAsync()
        {
            if (HttpContext.Session.GetString("Lang") == null)
            {
                HttpContext.Session.SetString("Lang", "English");
            }
            return Page();
        }

        public async Task<IActionResult> OnGetChatBotData()
        {
            var modelResponse = await _httpClient.GetAsync("ChatBot/GetData/", false).ConfigureAwait(false);
            if (modelResponse == "unauthorized") return RedirectToPage("/Index");
            var result = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<string>(modelResponse) : null;
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetChatBotChildData(int id)
        {
            var modelResponse = await _httpClient.GetAsync("ChatBot/GetChildData/" + id + "", false).ConfigureAwait(false);
            if (modelResponse == "unauthorized") return RedirectToPage("/Index");
            var result = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<string>(modelResponse) : null;
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetChatBotQuestionsData(int id)
        {
            var modelResponse = await _httpClient.GetAsync("ChatBot/GetQuestionsData/" + id + "", false).ConfigureAwait(false);
            if (modelResponse == "unauthorized") return RedirectToPage("/Index");
            var result = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<string>(modelResponse) : null;
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetChatBotAnswersData(int id)
        {
            var modelResponse = await _httpClient.GetAsync("ChatBot/GetAnswersData/" + id + "", false).ConfigureAwait(false);
            if (modelResponse == "unauthorized") return RedirectToPage("/Index");
            var result = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<string>(modelResponse) : null;
            return new JsonResult(result);
        }
    }
}