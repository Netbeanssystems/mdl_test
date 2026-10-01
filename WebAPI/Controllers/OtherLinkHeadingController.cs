using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class OtherLinkHeadingController : Controller
    {
        private readonly IDataService _dataService;

        public OtherLinkHeadingController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // POST: Categories/Create
        [HttpPost]

        public async Task<IActionResult> Create([FromBody] OtherLinkHeadingDTO modelHeadingDto)
        {
            if (modelHeadingDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService.OtherLinkHeading.Add(modelHeadingDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Create failed");
            return Ok(modelHeadingDto);
        }
        // GET Categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var categories = await _dataService.OtherLinkHeading.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Headings not found");
            return Ok(categories);
        }
        [AllowAnonymous]
        // GET Categories/GetCategoriesWithAll
        [HttpGet]
        public async Task<IActionResult> GetMenuHeadingsWithAll()
        {
            var Categories = await _dataService.OtherLinkHeading.GetCategoriesWithAll().ConfigureAwait(false);
            if (Categories == null || Categories.Count <= 0) return NotFound("Headings not found");
            return Ok(Categories);
        }

        [AllowAnonymous]
        [HttpGet("{Heading}")]
        public async Task<IActionResult> GetMenus([FromRoute] string Heading)
        {
            if (Heading == null) return BadRequest("Input not valid or null");
            var category = await _dataService.OtherLinkHeading.Get(Heading).ConfigureAwait(false);
            if (category == null) return NotFound("headings not found");
            return Ok(category);
        }

        // GET Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService.OtherLinkHeading.Get(id).ConfigureAwait(false);
            if (category == null) return NotFound("Menu headings not found");
            return Ok(category);
        }
        // PUT: Categories/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] OtherLinkHeadingDTO modelHeadingDto)
        {
            if (modelHeadingDto == null) return BadRequest("Input not valid or null");
            if (id != modelHeadingDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService.OtherLinkHeading.Update(modelHeadingDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Update failed");
            return Ok(modelHeadingDto);
        }

        // GET Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTest([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService.OtherLinkHeading.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (category == null) return NotFound("headings not found");
            return Ok(category);
        }

        // DELETE: Categories/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService.OtherLinkHeading.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditMenuContent([FromRoute] int id, [FromBody] OtherLinkHeadingDTO modelHeadingDto)
        {
            if (modelHeadingDto == null) return BadRequest("Input not valid or null");
            if (id != modelHeadingDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var Result = await _dataService.OtherLinkHeading.EditMenuContent(modelHeadingDto).ConfigureAwait(false);
            if (Result == null) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePriority([FromBody] List<OtherLinkListPriorityDTO> lstMenuDTOs)
        {
            int Result = await _dataService.OtherLinkHeading.UpdateMenus(lstMenuDTOs).ConfigureAwait(false);
            if (Result == 0) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }

        [AllowAnonymous]
        [HttpGet("{Heading?}")]
        public async Task<IActionResult> GetStockData([FromRoute] string Heading)
        {
            var stockResult = await _dataService.OtherLinkHeading.GetStockData(Heading).ConfigureAwait(false);
            if (stockResult == null) return NotFound("Stock Exchange Data  not found");
            return Ok(stockResult);
        }

        [AllowAnonymous]
        [HttpGet("{Heading}")]
        public async Task<IActionResult> GetStockDataHindi([FromRoute] string Heading)
        {
            var stockResult = await _dataService.OtherLinkHeading.GetStockDataHindi(Heading).ConfigureAwait(false);
            if (stockResult == null) return NotFound("Stock Exchange hindi Data  not found");
            return Ok(stockResult);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetStockExchange()
        {
            var stock = await _dataService.OtherLinkHeading.GetStockExchange().ConfigureAwait(false);
            if (stock == null || stock.Count <= 0) return NotFound("Stock Exchange not found");
            return Ok(stock);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetcorporateExchange()
        {
            var stock = await _dataService.OtherLinkHeading.GetcorporateExchange().ConfigureAwait(false);
            if (stock == null || stock.Count <= 0) return NotFound("Corporate data not found");
            return Ok(stock);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCommitment()
        {
            var stock = await _dataService.OtherLinkHeading.GetCommitment().ConfigureAwait(false);
            if (stock == null || stock.Count <= 0) return NotFound("Commitment data not found");
            return Ok(stock);
        }

        //[in-use]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetServiceCOCOs()
        {
            var stock = await _dataService.OtherLinkHeading.GetServiceCOCOs().ConfigureAwait(false);
            if (stock == null || stock.Count <= 0) return NotFound("COCOs data not found");
            return Ok(stock);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAnalystInstitutionalInvestors()
        {
            var stock = await _dataService.OtherLinkHeading.GetAnalystInstitutionalInvestors().ConfigureAwait(false);
            if (stock == null || stock.Count <= 0) return NotFound("Commitment data not found");
            return Ok(stock);
        }

        [AllowAnonymous]
        [HttpGet("{Type?}")]
        public async Task<IActionResult> GetHeading([FromRoute] string Type)
        {
            var stock = await _dataService.OtherLinkHeading.GetHeading(Type).ConfigureAwait(false);
            if (stock == null || stock.Count <= 0) return NotFound("Stock Exchange not found");
            return Ok(stock);
        }
        [AllowAnonymous]
        [HttpGet("{Type?}")]
        public async Task<IActionResult> GetHeadingForshareholding([FromRoute] string Type)
        {
            var stock = await _dataService.OtherLinkHeading.GetHeadingForshareholding(Type).ConfigureAwait(false);
            if (stock == null || stock.Count <= 0) return NotFound("Shareholding not found");
            return Ok(stock);
        }

        [AllowAnonymous]
        [HttpGet("{q}")]
        public async Task<IActionResult> GetSearchfromotherlinks([FromRoute] string q)
        {
            if (q == null) return BadRequest("Input not valid or null");
            var category = await _dataService.OtherLinkHeading.GetSearchfromotherlinks(q).ConfigureAwait(false);
            if (category == null) return NotFound("Headings not found");
            return Ok(category);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetcmsforCorporateGovernance()
        {
            var categories = await _dataService.OtherLinkHeading.GetcmsforCorporateGovernance().ConfigureAwait(false);
            if (categories == null) return NotFound("Data not found");
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getlastrecord()
        {
            var stock = await _dataService.OtherLinkHeading.Getlastrecord().ConfigureAwait(false);
            if (stock == null) return NotFound("Data not found");
            return Ok(stock);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getlastrecordshareprice()
        {
            var stock = await _dataService.OtherLinkHeading.Getlastrecordshareprice().ConfigureAwait(false);
            if (stock == null) return NotFound("Data not found");
            return Ok(stock);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getlastrecordanalystsinstitutional()
        {
            var stock = await _dataService.OtherLinkHeading.Getlastrecordanalystsinstitutional().ConfigureAwait(false);
            if (stock == null) return NotFound("Data not found");
            return Ok(stock);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getlastrecordstockexchange()
        {
            var stock = await _dataService.OtherLinkHeading.Getlastrecordstockexchange().ConfigureAwait(false);
            if (stock == null) return NotFound("Data not found");
            return Ok(stock);
        }

    }
}