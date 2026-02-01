using Application.Dtos;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class VendorController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;
        public VendorController(IDataService dataService, IConfiguration configuration)
        {
            _dataService = dataService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("{VendorCode}/{PANNo}")]
        public IActionResult GetVenderGSTInfo([FromRoute] string VendorCode, string PANNo)
        {
            int result = 0;
            string where = " tbl_VenderGST.Vender_Code=" + VendorCode + " AND tbl_VenderGST.Permanent_Account_Number='" + PANNo + "' AND tbl_VenderGST.Status='APP' AND tbl_VenderGST.Del_Sts='N' ";

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da1 = new SqlDataAdapter("usp_GetVenderGSTInfo", con);
                da1.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da1.SelectCommand.Parameters.AddWithValue("@where", where);
                da1.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    result = Convert.ToInt32(ds1.Tables[0].Rows[0]["id"].ToString());
                }
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{pkid}")]
        public IActionResult GetVenderGSTDetails([FromRoute] string pkid)
        {
            VendorGSTDataDTO vendorGSTDataDTO = new VendorGSTDataDTO();
            string where = " tbl_VenderGST.id=" + pkid;

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da1 = new SqlDataAdapter("usp_GetVenderGSTInfo", con);
                da1.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da1.SelectCommand.Parameters.AddWithValue("@where", where);
                da1.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    vendorGSTDataDTO.Id = Convert.ToInt32(ds1.Tables[0].Rows[0]["id"]);
                    vendorGSTDataDTO.VendorCode = ds1.Tables[0].Rows[0]["Vender_Code"].ToString();
                    vendorGSTDataDTO.VenderName = ds1.Tables[0].Rows[0]["Vender_Name"].ToString();
                    vendorGSTDataDTO.VenderAddress = ds1.Tables[0].Rows[0]["VenderAddress"].ToString();
                    vendorGSTDataDTO.Permanent_Account_Number = ds1.Tables[0].Rows[0]["Permanent_Account_Number"].ToString();
                    vendorGSTDataDTO.Email_ID = ds1.Tables[0].Rows[0]["Email_ID"].ToString();
                    vendorGSTDataDTO.GST_Number = ds1.Tables[0].Rows[0]["GST_Number"].ToString();
                    vendorGSTDataDTO.GST_FileName = ds1.Tables[0].Rows[0]["GST_FileName"].ToString();
                }
            }
            return Ok(vendorGSTDataDTO);
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public IActionResult UpdateVenderInfo([FromRoute] int id, [FromBody] VendorGSTPostDTO model)
        {
            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand da1 = new SqlCommand("usp_UpdateVenderInfo", con);
                da1.CommandType = System.Data.CommandType.StoredProcedure;
                da1.Parameters.AddWithValue("@id", model.Id);
                da1.Parameters.AddWithValue("@GST_Number", model.GST_Number);
                da1.Parameters.AddWithValue("@GST_FileName", model.GST_FileName);
                da1.Parameters.AddWithValue("@GST_Number_AddedIP", model.IP);
                da1.Parameters.AddWithValue("@msg", "");
                con.Open();
                da1.ExecuteNonQuery();
            }
            return Ok("Success");
        }
    }
}
