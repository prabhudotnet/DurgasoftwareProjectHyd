using checkboxDynamic.DataAccess;
using checkboxDynamic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace checkboxDynamic.Controllers
{
    public class CityController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DbHelper _dbHelper;



        public CityController(IConfiguration configuration, DbHelper dbHelper)
        {
            _config = configuration;
            _dbHelper = dbHelper;

        }

        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetCities(int districtId)
        {
            var city = _dbHelper.GetCities(districtId);
            return Json(city);
        }
        public string CityStoreOrder(string city)
        {
            string lowLtr = "";
            foreach (char letter in city)
            {
                lowLtr += char.ToLower(letter);
            }
            return lowLtr;
        }
        [HttpPost]
        public IActionResult CreateCity(string City,string cityId1)
        {
            try
            {
                int id = Convert.ToInt32(cityId1);
                string city = CityStoreOrder(City);
                string message = "SomeThing Went Wrong";
                string conStr = _config.GetConnectionString("MyApp")!;
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    //SqlCommand cmd = new SqlCommand($"Insert into Countries1 values('{country}')",conn);
                    SqlCommand cmd = new SqlCommand("InsGenCity", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@city", city);
                    cmd.Parameters.AddWithValue("@districtId", id);
                    cmd.Parameters.Add("@result", System.Data.SqlDbType.VarChar, 50).Direction = System.Data.ParameterDirection.Output;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    string res = cmd.Parameters["@result"].Value.ToString()!;
                    if (res == "Success")
                    {
                        message = $"City {city} Added Successfully!";
                    }
                    else
                        message = $"{res} While Adding {city}!";
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
        public IActionResult DeleteCity(string cityId)
        {
            try
            {
                int id = Convert.ToInt32(cityId);
               // string city = CityStoreOrder(City);
                string message = "SomeThing Went Wrong";
                string conStr = _config.GetConnectionString("MyApp")!;
                using (SqlConnection conn = new SqlConnection(conStr))
                {

                    //SqlCommand cmd = new SqlCommand("InsGenCity", conn);
                    //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    // cmd.Parameters.AddWithValue("@city", city);
                    // cmd.Parameters.AddWithValue("@districtId", id);
                    //cmd.Parameters.Add("@result", System.Data.SqlDbType.VarChar, 50).Direction = System.Data.ParameterDirection.Output;
                    SqlCommand cmd = new SqlCommand($"delete tbl_city where Id={id}",conn);
                    conn.Open();
                    int i = cmd.ExecuteNonQuery();
                    conn.Close();
                    if (i > 0)
                    {
                        message = "Deleted Successfully!";

                    }
     
                    else
                    {
                        message = "Cannot delete";
                    }
                        
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
