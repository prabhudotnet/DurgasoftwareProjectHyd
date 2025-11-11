using checkboxDynamic.DataAccess;
using checkboxDynamic.Models;
using checkboxDynamic.ViewModal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;


namespace checkboxDynamic.Controllers
{
    public class AdminController : Controller
    {
        private readonly DbHelper _dbHelper;
        private readonly IConfiguration _connectionString;
        public AdminController(DbHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _connectionString = configuration;
        }

        [HttpGet]
        public IActionResult AddInterest()
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                var viewModel = new ProjectViewModel
                {
                    AvailableQuestions = GetQuestions(),
                    AvailableInterest = GetInterest(),
                    AvailableGender = GetGenders(),
                    AvailableCountries = GetCountries()
                    
                };
                ViewBag.userList = userList();

                ViewBag.SecurityQtn = _dbHelper.GetQuestion();
                var interests = _dbHelper.GetAllInterests();
              
               
                return View(viewModel);
            }
            return RedirectToAction("Login","Account");
            
        }
        public List<SelectListItem> GetGenders()
        {
            string conStr = _connectionString.GetConnectionString("MyApp")!;
            List<Gender> genders = new List<Gender>();
            {
                using (var connection = new SqlConnection(conStr))
                {
                    var command = new SqlCommand("SELECT id, genderName FROM gender", connection);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            genders.Add(new Gender()
                            {
                                id = reader.GetInt32(0),
                                genderName = reader.GetString(1)
                            });
                        }
                    }
                }

            }
            return genders.Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.genderName
            }).ToList();
        }
        public List<SelectListItem> GetCountries()
        {
            string conStr = _connectionString.GetConnectionString("MyApp")!;
            List<Country> countries = new List<Country>();
            {
                using (var connection = new SqlConnection(conStr))
                {
                    var command = new SqlCommand("SELECT Id,Name FROM tbl_country", connection);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            countries.Add(new Country()
                            {
                                 Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

            }
            return countries.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
        [HttpPost]
        public IActionResult AddGender(string genderName)
        {
            string gender = genderName.ToLower();
            TempData["Msg"] = _dbHelper.AddGender(gender);

            return RedirectToAction("AddInterest");
        }
        [HttpPost]
        public IActionResult DeleteGender(string genderName)
        {
            string gender = genderName.ToLower();
            TempData["Msg"] = _dbHelper.RemoveGender(gender);

            return RedirectToAction("AddInterest");
        }
        private List<SelectListItem> GetInt()
        {
         
            var interest = new List<Interest>
        {
            new Interest { InterestId=1, InterestName="playing" },
            new Interest { InterestId = 2, InterestName ="jogging" },
            new Interest { InterestId= 3, InterestName = "Clothing" }
        };

            return interest.Select(c => new SelectListItem
            {
                Value = c.InterestId.ToString(),
                Text = c.InterestName
            }).ToList();
        }
        private List<SelectListItem> GetInterest()
        {
            string conStr = _connectionString.GetConnectionString("MyApp")!;
   
            List<Interest> interest1 = new List<Interest>();
            {
                using (var connection = new SqlConnection(conStr))
                {
                    var command = new SqlCommand("SELECT InterestId, InterestName FROM Interests", connection);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            interest1.Add(new Interest
                            {
                                InterestId = reader.GetInt32(0),
                                InterestName = reader.GetString(1)
                            });
                        }
                    }
                }
                return interest1.Select(c => new SelectListItem
                {
                    Value = c.InterestId.ToString(),
                    Text = c.InterestName
                }).ToList();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddInterest(string newInterestName)
        {
            if (!string.IsNullOrWhiteSpace(newInterestName))
            {
                string new_iName = newInterestName.ToLower();
                TempData["Msg"] = _dbHelper.AddNewInterest(new_iName);
            }
            return RedirectToAction("AddInterest");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteInterest(string newInterestName)
        {
            if (!string.IsNullOrWhiteSpace(newInterestName))
            {
                TempData["Msg"] = _dbHelper.DeleteInterest(newInterestName);
            }
            return RedirectToAction("AddInterest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuestion(string Question)
        {
            if (!string.IsNullOrWhiteSpace(Question))
            {
                try
                {
                    _dbHelper.AddNewQuestion(Question);
                    TempData["Msg"] = "Question Added Successfully!";

                }
                catch (Exception ex)
                {
                    TempData["Msg"] = "Cannot Add Question " + ex.Message;
                    return RedirectToAction("AddInterest");
                }
            }
            return RedirectToAction("AddInterest");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuestion(string QtnId)
        {
            if (!string.IsNullOrWhiteSpace(QtnId))
            {
                int id = Convert.ToInt32(QtnId);

                try
                {
                    _dbHelper.DeleteQuestion(id);
                    TempData["Msg"] = "Question Deleted Successfully!";

                }
                catch (Exception ex)
                {
                    TempData["Msg"] = "Cannot Delete Question Its Added By Users!";
                    return RedirectToAction("AddInterest");
                }
            }
            return RedirectToAction("AddInterest");
        }
        private List<SelectListItem> GetQuestions()
        {
            string conStr = _connectionString.GetConnectionString("MyApp")!;
            var interests = new List<SecurityQtn>();
            using (var connection = new SqlConnection(conStr))
            {
                var command = new SqlCommand("SELECT id, securityQuestion FROM securitYQuestion", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        interests.Add(new SecurityQtn
                        {
                            Id = reader.GetInt32(0),
                            question = reader.GetString(1)
                        });
                    }
                }
            }
            return interests.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.question
            }).ToList();

        }
        public IActionResult Index()
        {
            return View();
        }
        public List<string> userList()
        {
            string conStr = _connectionString.GetConnectionString("MyApp")!;
            List<string> userReg = new List<string>();
            using (SqlConnection conn=new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("userList", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                conn.Open();
                SqlDataAdapter sda= new SqlDataAdapter(cmd);
                DataTable dt=new DataTable();
                sda.Fill(dt);
                conn.Close();
                foreach (DataRow row in dt.Rows)
                {
                    if ((row["Id"]!=DBNull.Value)&&(row["Name"] != DBNull.Value)&&(row["country"] != DBNull.Value)&& (row["state"] != DBNull.Value)
                            && (row["district"] != DBNull.Value)&& (row["city"] != DBNull.Value)
                            && (row["photo"] != DBNull.Value)&& (row["Gender"] != DBNull.Value))
                    {
                        userReg.Add(row["Id"].ToString()!);
                        userReg.Add(row["Name"].ToString()!);
                        userReg.Add(row["Gender"].ToString()!);
                        userReg.Add(row["country"].ToString()!);
                        userReg.Add(row["state"].ToString()!);
                        userReg.Add(row["district"].ToString()!);
                        userReg.Add(row["city"].ToString()!);
                        userReg.Add(row["photo"].ToString()!);
                    }
                }
            }
            return userReg;
        }
        [HttpPost]
        public IActionResult DeleteUser(string userIdNumber)
        {
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images1");
            int id =Convert.ToInt32(userIdNumber);
            string img = _dbHelper.UserImg(id);
            string res= _dbHelper.DeleteUser(id);
            if (res == "Success")
            {
                if (img == null)
                {
                    TempData["Msg"] = "Image not avaialble User only Deleted Successfully!";
                    return RedirectToAction("AddInterest");
                }
                if (System.IO.File.Exists(savePath + "/" + img))
                {
                    System.IO.File.Delete(savePath + "/" + img);
                }

                TempData["Msg"] = "User Deleted Successfully!";
                return RedirectToAction("AddInterest");
            }
            TempData["Msg"] = "User Deleted Failed!";
            return RedirectToAction("AddInterest");

        }
    }
}
