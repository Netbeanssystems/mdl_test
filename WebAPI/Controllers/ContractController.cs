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
    public class ContractController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;
        public ContractController(IDataService dataService, IConfiguration configuration)
        {
            _dataService = dataService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetDocuments([FromRoute] int categoryId, string status)
        {
            List<DocumentModel> doclist = new List<DocumentModel>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  Tender_Date <= DATEADD(mm,-6,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  Tender_Date >= DATEADD(mm,-6,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da1 = new SqlDataAdapter("uspGetDocumentsbyCategory", con);
                da1.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da1.SelectCommand.Parameters.AddWithValue("@where", where);

                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds1.Tables[0].Rows)
                    {
                        DocumentModel str = new DocumentModel();
                        str.Document_Name = dr1["Document_Name"].ToString();
                        str.Document_Name_Hindi = dr1["document_name_hindi"].ToString();
                        str.Document_PDF = dr1["Document_PDF"].ToString();
                        str.Tender_id = dr1["Tender_id"].ToString();
                        str.Del_Sts = dr1["Del_Sts"].ToString();
                        doclist.Add(str);
                    }
                }
            }
            return Ok(doclist);
        }


        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetContracts([FromRoute] int categoryId, string status)
        {
            ContractDTO cont = new ContractDTO();
            string link = "";
            string lastUpdatedDate = ""; 

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("uspGetContractsDataDisplay", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Category_id", categoryId);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                for (int j = 1; j < ds.Tables.Count; j++)
                {
                    if (ds.Tables[j].Rows.Count > 0)
                    {
                        if (status == "archive")
                        {
                            if (ds.Tables[j].Rows[0]["Contract_year"].ToString() == DateTime.Now.Year.ToString() ||
                                ds.Tables[j].Rows[0]["Added_On"].ToString() == (DateTime.Now.AddMonths(6).Year - 1).ToString())
                            {
                                // Do nothing
                            }
                            else
                            {
                                link += "<h4>" + ds.Tables[j].Rows[0]["Contract_year"].ToString() + "</h4>";
                                link += "<ul class='pdf'>";
                                for (int i = 0; i < ds.Tables[j].Rows.Count; i++)
                                {
                                    link += "<li><a href=\"/app/writereaddata/contracts/" + ds.Tables[j].Rows[i]["Document_PDF"].ToString() + "\" target=\"_blank\">" + ds.Tables[j].Rows[i]["Document_Name"].ToString() + "</a></li>";
                                }
                                if (!string.IsNullOrEmpty(ds.Tables[j].Rows[0]["Notes"].ToString()))
                                {
                                    link += "<div class='vimpinfo'>Notes:" + ds.Tables[j].Rows[0]["Notes"].ToString() + "</div>";
                                }
                                link += "</ul>";
                                lastUpdatedDate = ds.Tables[j].Rows[0]["Added_On"].ToString(); // Store the last updated date.
                            }
                        }
                        else
                        {
                            if (ds.Tables[j].Rows[0]["Contract_year"].ToString() == DateTime.Now.Year.ToString() ||
                                ds.Tables[j].Rows[0]["Added_On"].ToString() == (DateTime.Now.AddMonths(6).Year - 1).ToString())
                            {
                                link += "<h4>" + ds.Tables[j].Rows[0]["Contract_year"].ToString() + "</h4>";
                                link += "<ul class='pdf'>";
                                for (int i = 0; i < ds.Tables[j].Rows.Count; i++)
                                {
                                    link += "<li><a href=\"/app/writereaddata/contracts/" + ds.Tables[j].Rows[i]["Document_PDF"].ToString() + "\" target=\"_blank\">" + ds.Tables[j].Rows[i]["Document_Name"].ToString() + "</a></li>";
                                }
                                if (!string.IsNullOrEmpty(ds.Tables[j].Rows[0]["Notes"].ToString()))
                                {
                                    link += "<div class='vimpinfo'>Notes:" + ds.Tables[j].Rows[0]["Notes"].ToString() + "</div>";
                                }
                                link += "</ul>";
                                lastUpdatedDate = ds.Tables[j].Rows[0]["Added_On"].ToString(); // Store the last updated date.
                            }
                        }
                    }
                }

                // Append the "Last Updated Date" at the end of the page.
                if (!string.IsNullOrEmpty(lastUpdatedDate))
                {
                    link += "<div style='display: flex; justify-content: flex-end; color: #00008B;'>";
                    link += "<span>Last Updated Date: </span>";
                    link += "<span style='text-align: right;'>" + lastUpdatedDate + "</span>";
                    link += "</div>";
                }

                cont.html = link;
            }
            return Ok(cont);
        }

        
        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetContracts1([FromRoute] int categoryId, string status)
        {
            ContractHindiDTO cont1 = new ContractHindiDTO();
            string link = "";
            string lastUpdatedDate = "";
            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("uspGetContractsDataDisplay", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Category_id", categoryId);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                for (int j = 1; j < ds.Tables.Count; j++)
                {
                    if (ds.Tables[j].Rows.Count > 0)
                    {
                        if (status == "archive")
                        {
                            if (ds.Tables[j].Rows[0]["Contract_year"].ToString() == DateTime.Now.Year.ToString() || ds.Tables[j].Rows[0]["Added_On"].ToString() == (DateTime.Now.AddMonths(6).Year - 1).ToString())
                            // (Convert.ToInt32(DateTime.Now.Year) - 1).ToString())
                            {

                            }
                            else
                            {
                                link += "<h4>" + ds.Tables[j].Rows[0]["Contract_year"].ToString() + "</h4>";
                                link += "<ul class='pdf'>";
                                for (int i = 0; i < ds.Tables[j].Rows.Count; i++)
                                {
                                    link += "<li><a href=\"/app/writereaddata/contracts/" + ds.Tables[j].Rows[i]["Document_PDF"].ToString() + "\" target=\"_blank\" >" + ds.Tables[j].Rows[i]["Document_Name_Hindi"].ToString() + "</a></li>";
                                }
                                if (!string.IsNullOrEmpty(ds.Tables[j].Rows[0]["NotesHindi"].ToString()))
                                {
                                    link += "<div class='vimpinfo'>Notes:" + ds.Tables[j].Rows[0]["NotesHindi"].ToString() + "</div>";
                                }
                                link += "</ul>";
                                lastUpdatedDate = ds.Tables[j].Rows[0]["Added_On"].ToString();
                            }
                        }
                        else
                        {
                            if (ds.Tables[j].Rows[0]["Contract_year"].ToString() == DateTime.Now.Year.ToString() || ds.Tables[j].Rows[0]["Added_On"].ToString() == (DateTime.Now.AddMonths(6).Year - 1).ToString())
                            // (Convert.ToInt32(DateTime.Now.Year) - 1).ToString())
                            {
                                link += "<h4>" + ds.Tables[j].Rows[0]["Contract_year"].ToString() + "</h4>";
                                link += "<ul class='pdf'>";
                                for (int i = 0; i < ds.Tables[j].Rows.Count; i++)
                                {
                                    link += "<li><a href=\"/app/writereaddata/contracts/" + ds.Tables[j].Rows[i]["Document_PDF"].ToString() + "\" target=\"_blank\" >" + ds.Tables[j].Rows[i]["Document_Name_Hindi"].ToString() + "</a></li>";
                                }
                                if (!string.IsNullOrEmpty(ds.Tables[j].Rows[0]["NotesHindi"].ToString()))
                                {
                                    link += "<div class='vimpinfo'>Notes:" + ds.Tables[j].Rows[0]["NotesHindi"].ToString() + "</div>";
                                }
                                link += "</ul>";
                                lastUpdatedDate = ds.Tables[j].Rows[0]["Added_On"].ToString();
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(lastUpdatedDate))
                {
                    link += "<div style='display: flex; justify-content: flex-end; color: #00008B;'>";
                    link += "<span>अंतिम अद्यतन तिथि : </span>";
                    link += "<span style='text-align: right;'>" + lastUpdatedDate + "</span>";
                    link += "</div>";
                }
                cont1.html = link;
            }
            return Ok(cont1);
        }

    }
}
