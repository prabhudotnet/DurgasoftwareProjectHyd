using checkboxDynamic.DataAccess;
using checkboxDynamic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace checkboxDynamic.Controllers
{
    public class DistrictController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DbHelper _dbHelper;



        public DistrictController(IConfiguration configuration, DbHelper dbHelper)
        {
            _config = configuration;
            _dbHelper = dbHelper;

        }

        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetDistricts(int stateId)
        {
            var Districts = _dbHelper.GetDistrict(stateId);
            return Json(Districts);
        }
        public string DistrictStoreOrder(string district)
        {
            string lowLtr = "";
            foreach (char letter in district)
            {
                lowLtr += char.ToLower(letter);
            }
            return lowLtr;
        }
        [HttpPost]
        public IActionResult CreateDistrict(string District, string stateId)
        {
            try
            {
                int id = Convert.ToInt32(stateId);
                string district= DistrictStoreOrder(District);
                string message = "SomeThing Went Wrong";
                string conStr = _config.GetConnectionString("MyApp")!;
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    //SqlCommand cmd = new SqlCommand($"Insert into Countries1 values('{country}')",conn);
                    SqlCommand cmd = new SqlCommand("InsGenDistrict", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@district", district);
                    cmd.Parameters.AddWithValue("@stateId", id);
                    cmd.Parameters.Add("@result", System.Data.SqlDbType.VarChar, 50).Direction = System.Data.ParameterDirection.Output;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    string res = cmd.Parameters["@result"].Value.ToString()!;
                    if (res == "Success")
                    {
                        message = $"District {district} Added Successfully!";
                    }
                    else
                        message = $"{res} While Adding {district}!";
                }
                TempData["Msg"] = message;
            }
            catch (Exception ex)
            {
                TempData["Msg"] = ex.Message;
                return RedirectToAction("AddInterest", "Admin");

            }
            return RedirectToAction("AddInterest", "Admin");
        }
        
        [HttpPost]
        public IActionResult DeleteDistrict(string districtId1)
        {
            try
            {
                int id = Convert.ToInt32(districtId1);
                //string district = DistrictStoreOrder(District);
                string message = "SomeThing Went Wrong";
                string conStr = _config.GetConnectionString("MyApp")!;
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    //SqlCommand cmd = new SqlCommand($"Insert into Countries1 values('{country}')",conn);
                    SqlCommand cmd = new SqlCommand("DelDistrict", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@districtId", id);
                    cmd.Parameters.Add("@result", System.Data.SqlDbType.VarChar, 50).Direction = System.Data.ParameterDirection.Output;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    string res = cmd.Parameters["@result"].Value.ToString()!;
                    if (res == "Success")
                    {
                        message = $"District deleted Successfully!";
                    }
                    else
                        message = $"{res}!";
                }
                TempData["Msg"] = message;
            }
            catch (Exception ex)
            {
                TempData["Msg"] = ex.Message;
                return RedirectToAction("AddInterest", "Admin");

            }
            return RedirectToAction("AddInterest", "Admin");
        }
    }
}
