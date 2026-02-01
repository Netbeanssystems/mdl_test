using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class StacsController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;
        public StacsController(IDataService dataService, IConfiguration configuration)
        {
            _dataService = dataService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}")]
        public IActionResult GetStacs([FromRoute] int categoryId)
        {
            CommanStacDTO common = new CommanStacDTO();
            List<StacsDTO> stacs = new List<StacsDTO>();
            List<FormatDTO> formats = new List<FormatDTO>();
            List<OthersDTO> others = new List<OthersDTO>();

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da1 = new SqlDataAdapter("uspGetStacsDataforEdit", con);
                da1.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da1.SelectCommand.Parameters.AddWithValue("@Category_id", categoryId);
                da1.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[1].Rows)
                    {
                        StacsDTO str = new StacsDTO();
                        str.DocumentName = dr1["Document_Name"].ToString();
                        str.DocumentNameHindi = dr1["doc_hindi_name"].ToString();
                        str.DocumentPDF = dr1["Document_PDF"].ToString();
                        str.Added_on = Convert.ToDateTime(dr1["Added_on"]);
                        stacs.Add(str);
                    }
                }

                if (ds1.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[2].Rows)
                    {
                        FormatDTO str = new FormatDTO();
                        str.DocumentName = dr1["Document_Name"].ToString();
                        str.DocumentNameHindi = dr1["doc_hindi_name"].ToString();
                        str.DocumentPDF = dr1["Document_PDF"].ToString();
                        formats.Add(str);
                    }
                }

                if (ds1.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[3].Rows)
                    {
                        OthersDTO str = new OthersDTO();
                        str.DocumentName = dr1["Document_Name"].ToString();
                        str.DocumentNameHindi = dr1["doc_hindi_name"].ToString();
                        str.DocumentPDF = dr1["Document_PDF"].ToString();
                        others.Add(str);
                    }
                }

                common.stacs = stacs;
                common.formats = formats;
                common.others = others;
            }
            return Ok(common);
        }
    }
}
