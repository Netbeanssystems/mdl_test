using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class WhatsNewController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;
        public WhatsNewController(IDataService dataService, IConfiguration configuration)
        {
            _dataService = dataService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WhatsNewDTO NewsDto)
        {
            if (NewsDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var categoryDto = await _dataService._whatsNew.Add(NewsDto).ConfigureAwait(false);
            if (categoryDto == null) return BadRequest("Create failed");
            return Ok(NewsDto);
        }

        //---------------------------------------Old 
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _dataService._whatsNew.Get().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("News not found");
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetFooterDate()
        {
            List<FooterDateDTO> FooterDate = new List<FooterDateDTO>();

            string CS = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("usp_GetFooterDate", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds.Tables[0].Rows)
                    {
                        FooterDateDTO Footer = new FooterDateDTO();
                        Footer.ID = Convert.ToInt32(dr1["ID"]);
                        Footer.UpdatedOn = Convert.ToDateTime(dr1["UpdatedOn"]);
                        Footer.HeadingName = dr1["HeadingName"].ToString();
                        FooterDate.Add(Footer);
                    }
                }

            }
            return Ok(FooterDate);
        }


        //----------------------------Karn 11Dec 2023--------
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllWhatsNew()
        {
            var categories = await _dataService._whatsNew.GetAllWhats().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("News not found");
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> MostViewed()
        {
            var categories = await _dataService._whatsNew.MostViewed().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Most Viewed not found");
            return Ok(categories);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> WhatsNew()
        {
            var categories = await _dataService._whatsNew.WhatsNew().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("What's New not found");
            return Ok(categories);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> MinistryIndustryUpdates()
        {
            var categories = await _dataService._whatsNew.MinistryIndustryUpdates().ConfigureAwait(false);
            if (categories == null || categories.Count <= 0) return NotFound("Ministry Industry Updates not found");
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{Heading}")]
        public async Task<IActionResult> GetMenus([FromRoute] string Heading)
        {
            if (Heading == null) return BadRequest("Input not valid or null");
            var category = await _dataService._whatsNew.Get(Heading).ConfigureAwait(false);
            if (category == null) return NotFound("News Heading not found");
            return Ok(category);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getorderdesc([FromRoute] string Heading)
        {
            var category = await _dataService._whatsNew.Getorderdesc().ConfigureAwait(false);
            if (category == null) return NotFound("News Heading not found");
            return Ok(category);
        }

        [AllowAnonymous]
        // GET Categories/GetCategoriesWithAll
        [HttpGet]
        public async Task<IActionResult> GetMenuHeadingsWithAll()
        {
            var Categories = await _dataService._whatsNew.GetCategoriesWithAll().ConfigureAwait(false);
            if (Categories == null || Categories.Count <= 0) return NotFound("News Headings not found");
            return Ok(Categories);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var newProject = await _dataService._whatsNew.Get(id).ConfigureAwait(false);
            if (newProject == null) return NotFound("New Projects not found");
            return Ok(newProject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] WhatsNewDTO NewsDto)
        {
            if (NewsDto == null) return BadRequest("Input not valid or null");
            if (id != NewsDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newProjectDto = await _dataService._whatsNew.Update(NewsDto).ConfigureAwait(false);
            if (newProjectDto == null) return BadRequest("Update failed");
            return Ok(NewsDto);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTest([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var category = await _dataService._whatsNew.GetCategoriesWithAllTest(id).ConfigureAwait(false);
            if (category == null) return NotFound("News headings not found");
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id <= 0) return BadRequest("Input not valid or null");
            var rowsAffected = await _dataService._whatsNew.Remove(id).ConfigureAwait(false);
            if (rowsAffected > 0) return Ok(rowsAffected);
            return BadRequest("Delete failed");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditMenuContent([FromRoute] int id, [FromBody] WhatsNewDTO NewsDto)
        {
            if (NewsDto == null) return BadRequest("Input not valid or null");
            if (id != NewsDto.Id) return BadRequest("Invalid Id");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var Result = await _dataService._whatsNew.EditMenuContent(NewsDto).ConfigureAwait(false);
            if (Result == null) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePriority([FromBody] List<NewsListPriorityDto> lstMenuDTOs)
        {
            int Result = await _dataService._whatsNew.UpdateMenus(lstMenuDTOs).ConfigureAwait(false);
            if (Result == 0) return BadRequest("Update failed");
            return Ok("Successfully Add");
        }
    }
}
