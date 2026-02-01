using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite.Pages.Stacs
{
    public class MDC_StacsModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public MDC_StacsModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty] public CommanStacDTO cont { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("Stacs/GetStacs", false, 7).ConfigureAwait(false);
            cont = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<CommanStacDTO>(modelResponse) : null;

            if (cont.stacs.Count > 0 && cont != null)
            {
                cont.stacs = cont.stacs.OrderByDescending(s => s.Added_on).ToList();
                HttpContext.Session.SetString("LastDate", cont.stacs[0].Added_on.ToString());
            }

            return Page();
        }
    }
}
