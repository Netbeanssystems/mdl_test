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
    public class TendersController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;
        public TendersController(IDataService dataService, IConfiguration configuration)
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
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
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
        public IActionResult GetINDNotifications([FromRoute] int categoryId, string status)
        {
            List<INDNotificationDTO> list = new List<INDNotificationDTO>();


            string where = "";

            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        INDNotificationDTO notif = new INDNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.EOINo = dr["Tender_No"].ToString();
                        notif.EOINoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.EOIDate = dr["Tender_Date"].ToString();
                        notif.EOICloseDate = dr["TenderClosingDate"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());
                        notif.Added_on = Convert.ToDateTime(dr["Added_on"]); //Afroz
                        list.Add(notif);
                    }
                }
            }
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetSBMPNotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";

            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.LinkURL = dr["LinkURL"].ToString();
                        notif.LinkName = dr["LinkName"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.Added_on = Convert.ToDateTime(dr["Added_on"]);
                        list.Add(notif);
                    }
                }
            }
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetSBPMNotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }


            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.LinkURL = dr["LinkURL"].ToString();
                        notif.LinkName = dr["LinkName"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.Added_on = Convert.ToDateTime(dr["Added_on"]);
                        list.Add(notif);
                    }
                }
            }
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetSBONotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.LinkURL = dr["LinkURL"].ToString();
                        notif.LinkName = dr["LinkName"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.Added_on = Convert.ToDateTime(dr["Added_on"]); //Afroz
                        list.Add(notif);
                    }
                }
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetSubmarineNotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.LinkURL = dr["LinkURL"].ToString();
                        notif.LinkName = dr["LinkName"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.Added_on = Convert.ToDateTime(dr["Added_on"]);
                        list.Add(notif);
                    }
                }
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetTechnicalNotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.LinkURL = dr["LinkURL"].ToString();
                        notif.LinkName = dr["LinkName"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.Added_on = Convert.ToDateTime(dr["Added_on"]); //Afroz
                        list.Add(notif);
                    }
                }
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetInfraProjNotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.LinkURL = dr["LinkURL"].ToString();
                        notif.LinkName = dr["LinkName"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.Added_on = Convert.ToDateTime(dr["Added_on"]);
                        list.Add(notif);
                    }
                }
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetMDCNotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.LinkURL = dr["LinkURL"].ToString();
                        notif.LinkName = dr["LinkName"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        list.Add(notif);
                    }
                }
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetEstateNotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.Added_on= Convert.ToDateTime(dr["Added_on"]); //AfrozGetDocuments
                        list.Add(notif);
                    }
                }
            }

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId}/{status}")]
        public IActionResult GetITProcNotifications([FromRoute] int categoryId, string status)
        {
            List<SBMPNotificationDTO> list = new List<SBMPNotificationDTO>();

            string where = "";
            if (status == "archive")
            {
                where = " tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate <= DATEADD(dd,-180,GETDATE()) ";
            }
            else
            {
                where = "tbl_Notifications.Del_Sts='N' and tbl_Notifications.Category_id=" + categoryId + " and  TenderClosingDate >= DATEADD(dd,-180,GETDATE()) ";
            }

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("USP_GetNotificationsList", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SBMPNotificationDTO notif = new SBMPNotificationDTO();

                        notif.Description = dr["Work_Description"].ToString();
                        notif.DescriptionHindi = dr["work_desc_hindi"].ToString();
                        notif.TenderNo = dr["Tender_No"].ToString();
                        notif.TenderDate = dr["Tender_Date"].ToString();
                        notif.TenderCloseDate = dr["TenderClosingDate"].ToString();
                        notif.TenderFee = dr["Tender_Fee"].ToString();
                        notif.EMD = dr["EMD"].ToString();
                        notif.Tender_Id = Convert.ToInt32(dr["Tender_id"].ToString());

                        notif.TenderNumberMoreInfo = dr["TenderNumberMoreInfo"].ToString();
                        notif.Updated_Work_Description = dr["Updated_Work_Description"].ToString();
                        notif.TenderClosingTime = dr["TenderClosingTime"].ToString();
                        notif.UpdatedTenderClosingDate = dr["UpdatedTenderClosingDate"].ToString();
                        notif.LinkURL = dr["LinkURL"].ToString();
                        notif.LinkName = dr["LinkName"].ToString();
                        notif.EMDHindi = dr["EmdHindi"].ToString();
                        notif.TenderClosingTimeHindi = dr["TenderClosingTimeHindi"].ToString();
                        notif.TenderNoHindi = dr["Tender_No_Hindi"].ToString();
                        notif.Added_on = Convert.ToDateTime(dr["Added_on"]);
                        list.Add(notif);
                    }
                }
            }

            return Ok(list);
        }
    }
}
