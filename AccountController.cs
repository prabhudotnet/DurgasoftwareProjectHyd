using checkboxDynamic.DataAccess;
using checkboxDynamic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace checkboxDynamic.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbHelper _dbHelper;
        private readonly IConfiguration _connectionString;

       public AccountController(DbHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _connectionString = configuration;
        }
        [HttpGet]
        public IActionResult Register()
        {
            var interestsFromDb = _dbHelper.GetAllInterests();
            ViewBag.Questions = _dbHelper.GetQuestion();
            ViewBag.genders = _dbHelper.GetGenders();
            ViewBag.Countries = _dbHelper.GetCountries();
            var model = new UserRegistrationViewModel
            {
                Interests = interestsFromDb.Select(i => new InterestViewModel
                {
                    Id = i.InterestId,
                    Name = i.InterestName,
                    IsChecked = false
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserRegistrationViewModel model, IFormFile imageFile)
        {
            string res = "";
            if (ModelState.IsValid)
            {
                if (imageFile != null || imageFile!.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images1");
                    res = _dbHelper.RegisterUser(model, imageFile);
                    if (res == "Success")
                    {
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        var filePath = Path.Combine(savePath, fileName);
                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            imageFile.CopyTo(fs);

                        }

                    }
                    else
                    {
                        TempData["result"] = res;
                        return RedirectToAction("Register");
                    }

                }

                TempData["result"] = "User Register Successfully!";
                //return View(model);
                return RedirectToAction("Register");
                //return RedirectToAction("RegistrationSuccess");
            }


            TempData["result"] = "Invalid Input User Must Provide All Inputs!";
            return RedirectToAction("Register");

   
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email,string pwd)
        {

            if (email != null && pwd !=null)
            {
                string name = _dbHelper.LoginUser(email,pwd);
                if (name == "DurgaAdmin@gmail.com")
                {
                    HttpContext.Session.SetString("user", name);
                    return RedirectToAction("AddInterest", "Admin");
                }
                else if (name != null)
                {
                    HttpContext.Session.SetString("name", name);
                    return RedirectToAction("UserDash");
                }
                else
                    ViewBag.result = "Invalid Credentials!";
                return View();

            }
            ViewBag.result = "Invalid Inputs!";
            return View();


        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public List<InterestViewModel> GetAllInterests1()
        {

            string conStr = _connectionString.GetConnectionString("MyApp")!;
            var interests = new List<InterestViewModel>();
            using (var connection = new SqlConnection(conStr))
            {
                var command = new SqlCommand("SELECT InterestId, InterestName FROM Interests", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        interests.Add(new InterestViewModel()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return interests;
        }

        [HttpGet]
        public IActionResult UserDash()
        {
            
            if (HttpContext.Session.GetString("name") != null)
            {
                string name = HttpContext.Session.GetString("name")!;
                // ViewBag.message = _dbHelper.GetUsers(name);

                var interestsFromDb = _dbHelper.GetAllInterests();

                var model = new UserRegistrationViewModel
                {
                    Interests = interestsFromDb.Select(i => new InterestViewModel
                    {
                        Id = i.InterestId,
                        Name = i.InterestName,
                        IsChecked = false
                    }).ToList()

                };
                

                ViewBag.Questions = _dbHelper.GetQuestion();
                ViewBag.genders = _dbHelper.GetGenders();
                ViewBag.Countries = _dbHelper.GetCountries();
                var user = _dbHelper.GetUsers(name);
                if (user == null)
                {
                    return null!;
                }
                // var availableInterests = _dbHelper.GetAllInterests();
                var availableInterests = GetAllInterests1();
                foreach (var interest in availableInterests)
                {
                    if (user.Interests.Any(i => i.Id == interest.Id))
                    {
                        interest.IsChecked = true;
                    }
                }
                user.Interests = availableInterests;
                return View(user);
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update_User(UserRegistrationViewModel model, IFormFile? imageFile)
        {
            var fileName = "";
            string? oldImage = model.Parts_img;
          
            if (ModelState.IsValid)
            {
                var user = _dbHelper.Update_User(model, imageFile);
                if (user == "Success")
                {
                    if (imageFile != null)
                    {
                        if (imageFile.Length > 0)
                        {
                            fileName = Path.GetFileName(imageFile.FileName);
                            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images1");
                            if (!Directory.Exists(savePath))
                            {
                                Directory.CreateDirectory(savePath);
                            }
                            var filePath = Path.Combine(savePath, fileName);
                            using (var fs = new FileStream(filePath, FileMode.Create))
                            {
                                imageFile.CopyTo(fs);
                                if (System.IO.File.Exists(savePath + "/" + oldImage))
                                { 
                                    System.IO.File.Delete(savePath + "/" + oldImage);
                                }
                            }
                        }
                    }
                }
                else
                {
                    TempData["result"] = user;
                    return RedirectToAction("UserDash");
                }
                TempData["result"] = "Profile Update Successfully!";
                //return View(model);
                return RedirectToAction("UserDash");
                //return RedirectToAction("RegistrationSuccess");
            }
            TempData["result"] = "Invalid Input User Must Provide All Inputs!";
            return RedirectToAction("UserDash");
        }
        public IActionResult RegistrationSuccess()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        
        
        [HttpPost] 
        public JsonResult CheckEmail([FromBody] EmailInput input)
       {
            var result = _dbHelper.CheckEmailExists(input.Email!);
            return Json(result);
        }
        public class EmailInput
        {
            public string? Email { get; set; }
        }

    }
}




