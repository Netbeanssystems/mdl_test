using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WebSite.Pages.Contracts
{
    public class MDC_ContractsModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        public MDC_ContractsModel(IHttpClientService httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty] public ContractDTO cont { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var modelResponse = await _httpClient.GetAsync("Contract/GetContracts", false, 7, "current").ConfigureAwait(false);
            cont = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<ContractDTO>(modelResponse) : null;
            return Page();
        }

        public async Task<IActionResult> OnGetBindSelectdata(string status)
        {
            var modelResponse = await _httpClient.GetAsync("Contract/GetContracts", false, 7, status).ConfigureAwait(false);
            cont = !string.IsNullOrEmpty(modelResponse) ? JsonConvert.DeserializeObject<ContractDTO>(modelResponse) : null;

            return new PartialViewResult
            {
                ViewName = "_TenderContractPartial",
                ViewData = new ViewDataDictionary<ContractDTO>(ViewData, cont)
            };
        }
    }
}
