using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatBotController : Controller
    {
        private readonly IConfiguration _configuration;

        public ChatBotController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            List<ParentMenuModel> parentMenus = new();
            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Get_parentMenu";
                con.Open();
                using DataTable dt = new();
                dt.Load(cmd.ExecuteReader());
                foreach (DataRow rd in dt.Rows)
                {
                    ParentMenuModel parentMenuModel = new()
                    {
                        Id = Convert.ToInt32(rd["Id"]),
                        P_MenuName = Convert.ToString(rd["MenuNmae"]),
                        Urls = Convert.ToString(rd["Urls"])
                    };
                    parentMenus.Add(parentMenuModel);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return error status code and message
            }

            // Serialize the parentMenus list to JSON
            var json = JsonConvert.SerializeObject(parentMenus);

            // Return JSON response
            return Ok(json);
        }
        [HttpGet("{Id}")]
        public IActionResult ChildData(int Id)
        {
            List<ChildMenuModel> childMenus = new();
            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Get_ChildMenu";
                cmd.Parameters.Add(new SqlParameter("@Id", Id));
                con.Open();
                using DataTable dt = new();
                dt.Load(cmd.ExecuteReader());
                foreach (DataRow rd in dt.Rows)
                {
                    ChildMenuModel childMenuModel = new()
                    {
                        ChildMenuId = Convert.ToInt32(rd["C_Id"]),
                        ParantMenuId = Convert.ToInt32(rd["P_Id"]),
                        ChildMenu = Convert.ToString(rd["ChildMenu"]),
                        Urls = Convert.ToString(rd["Urls"])
                    };
                    childMenuModel.UrlsMSG = "Please click on the following link to " + childMenuModel.ChildMenu;
                    if (childMenuModel.ChildMenu != "Meter Reading")
                    {
                        childMenus.Add(childMenuModel);
                    }
                    childMenuModel = new ChildMenuModel();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return error status code and message
            }

            // Serialize the parentMenus list to JSON
            var json = JsonConvert.SerializeObject(childMenus);

            // Return JSON response
            return Ok(json);
        }
        [HttpGet("{Id}")]
        public IActionResult GetChildData(int Id)
        {
            List<ChildMenuModel> childMenus = new();
            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Get_ChildData";
                cmd.Parameters.Add(new SqlParameter("@Id", Id));
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    ChildMenuModel childMenu = new()
                    {
                        ChildMenuId = Convert.ToInt32(rd["C_Id"]),
                        ParantMenuId = Convert.ToInt32(rd["P_Id"]),
                        ChildMenu = Convert.ToString(rd["ChildMenu"]),
                    };
                    childMenus.Add(childMenu);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return error status code and message
            }

            // Serialize the parentMenus list to JSON
            var json = JsonConvert.SerializeObject(childMenus);

            // Return JSON response
            return Ok(json);
        }
        [HttpGet("{Id}")]
        public IActionResult GetQuestionsData(int Id)
        {
            List<QuestionsModel> questions = new();
            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Get_QuesionsData";
                cmd.Parameters.Add(new SqlParameter("@Id", Id));
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    QuestionsModel question = new()
                    {
                        Q_Id = Convert.ToInt32(rd["Q_Id"]),
                        P_Id = Convert.ToInt32(rd["P_Id"]),
                        Questions = Convert.ToString(rd["Questions"]),
                    };
                    questions.Add(question);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return error status code and message
            }

            // Serialize the parentMenus list to JSON
            var json = JsonConvert.SerializeObject(questions);

            // Return JSON response
            return Ok(json);
        }
        [HttpGet("{Id}")]
        public IActionResult GetAnswersData(int Id)
        {
            List<AnswersModel> answers = new();
            try
            {
                using SqlConnection con = new(_configuration.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Get_AnswersData";
                cmd.Parameters.Add(new SqlParameter("@Id", Id));
                con.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    AnswersModel answer = new()
                    {
                        A_Id = Convert.ToInt32(rd["A_Id"]),
                        Q_Id = Convert.ToInt32(rd["Q_Id"]),
                        Answers = Convert.ToString(rd["Answers"]),
                    };
                    answers.Add(answer);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return error status code and message
            }

            // Serialize the parentMenus list to JSON
            var json = JsonConvert.SerializeObject(answers);

            // Return JSON response
            return Ok(json);
        }
    }

    public class ChildMenuModel
    {
        public int ChildMenuId { get; set; }
        public int ParantMenuId { get; set; }
        public string ChildMenu { get; set; }
        public string Urls { get; set; }
        public string UrlsMSG { get; set; }
    }
    public class ParentMenuModel
    {
        public int Id { get; set; }
        public string P_MenuName { get; set; }
        public string Urls { get; set; }
    }
    public class QuestionsModel
    {
        public int Q_Id { get; set; }
        public int P_Id { get; set; }
        public string Questions { get; set; }
    }
    public class AnswersModel
    {
        public int A_Id { get; set; }
        public int Q_Id { get; set; }
        public string Answers { get; set; }
    }
    public class UserId
    {
        public int Id { get; set; }
    }
}