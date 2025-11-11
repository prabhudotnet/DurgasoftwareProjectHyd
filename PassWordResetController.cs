using checkboxDynamic.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace checkboxDynamic.Controllers
{
    public class PassWordResetController : Controller
    {

        private readonly DbHelper _dbHelper;
        private readonly IConfiguration _connectionString;

        public PassWordResetController(DbHelper dbHelper, IConfiguration configuration)
        {
            _dbHelper = dbHelper;
            _connectionString = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPwd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPwd(string Question,string email)
        {
            if (Question != null)
            {
                var result = _dbHelper.PassWordReset(Question,email);
                if (result!=null)
                {
                    return View(result);
                }
                
            }
            TempData["LoginMsg"] = "Invalid Data";
            return RedirectToAction("Login", "Account");
        }

        
        [HttpPost]
        public IActionResult UpdatePassWord(string email,string pwd)
        {
            if (email != null && pwd !=null)
            {
                _dbHelper.UpdatePassWord(email,pwd);
                TempData["result"] = "PassWord Updates Successfully!";
                return RedirectToAction("ResetPwd");
            }
            TempData["result"] = "Invalid Data";
            return RedirectToAction("ResetPwd");
        }
    }
}
