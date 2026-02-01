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
    public class CareerController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;
        public CareerController(IDataService dataService, IConfiguration configuration)
        {
            _dataService = dataService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet("{where}")]
        public IActionResult GetCareers([FromRoute] string where)
        {
            List<CareerDTO> carlist = new List<CareerDTO>();

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("usp_GetCareerJobsByCareer_Cat_id", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds.Tables[0].Rows)
                    {
                        CareerDTO car = new CareerDTO();
                        car.DatePosting = dr1["Date_of_Posting"].ToString();
                        car.AdvRefNo = dr1["AdvRefNumber"].ToString();
                        car.Post = dr1["Job_Post"].ToString();
                        car.Details = dr1["Job_Details"].ToString();
                        car.DetailsHindi = dr1["Hindi_job_Detail"].ToString();//akash
                        car.PostHindi = dr1["HindiPost"].ToString();//akash
                        car.AdvRefNumberHindi = dr1["AdvRefNumberHindi"].ToString();//akash
                        car.ClosingDate = dr1["Closingdate"].ToString();//akash
                        car.Link = dr1["LinkPath"].ToString();
                        car.FileName = dr1["FileName"].ToString();
                        car.Added_on = Convert.ToDateTime(dr1["Added_on"]);
                        carlist.Add(car);
                    }
                }
            }
            return Ok(carlist);
        }

        [AllowAnonymous]
        [HttpGet("{where}")]
        public IActionResult GetNonCareers([FromRoute] string where)
        {
            List<NonCareerDTO> carlist = new List<NonCareerDTO>();

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("usp_GetCareerJobsByCareer_Cat_id", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds.Tables[0].Rows)
                    {
                        NonCareerDTO car = new NonCareerDTO();
                        car.DatePosting = dr1["Date_of_Posting"].ToString();
                        car.AdvRefNo = dr1["AdvRefNumber"].ToString();
                        car.Link = dr1["LinkPath"].ToString();
                        car.FileName = dr1["FileName"].ToString();
                        car.ClosingDate = dr1["Closingdate"].ToString();//akash
                        car.AdvRefNumberHindi = dr1["AdvRefNumberHindi"].ToString();//akash
                        car.Added_on = Convert.ToDateTime(dr1["Added_on"]);
                        carlist.Add(car);
                    }
                }

            }
            return Ok(carlist);
        }

        [AllowAnonymous]
        [HttpGet("{where}")]
        public IActionResult GetExCareers([FromRoute] string where)
        {
            List<RetireCareerDTO> carlist = new List<RetireCareerDTO>();

            string CS = _configuration.GetConnectionString("MDLVendorconnect");
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlDataAdapter da = new SqlDataAdapter("usp_GetExEmployeeCareersByCatID", con);
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@where", where);
                da.SelectCommand.Parameters.AddWithValue("@msg", "");

                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr1 in ds.Tables[0].Rows)
                    {
                        RetireCareerDTO car = new RetireCareerDTO();
                        car.DatePosting = dr1["Ex_Employee_Date_of_Posting"].ToString();
                        car.Notification = dr1["Ex_Employee_Notification"].ToString();
                        car.NotificationHindi = dr1["ex_emp_noti_hindi"].ToString();
                        car.Link = dr1["Ex_Employee_LinkPath"].ToString();
                        car.ClosingDate = dr1["Closingdate"].ToString();//akash
                        car.FileName = dr1["Ex_Employee_FileName"].ToString();
                        car.Added_on = Convert.ToDateTime(dr1["Added_on"]);
                        carlist.Add(car);
                    }
                }

            }
            return Ok(carlist);
        }
    }
}