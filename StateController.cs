using checkboxDynamic.DataAccess;
using checkboxDynamic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace checkboxDynamic.Controllers
{
    public class StateController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DbHelper _dbHelper;



        public StateController(IConfiguration configuration, DbHelper dbHelper)
        {
            _config = configuration;
            _dbHelper = dbHelper;

        }

        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetStates(int countryId)
        {
            var States = _dbHelper.GetStates(countryId);
            return Json(States);
        }
        public string StateStoreOrder(string country)
        {
            string lowLtr = "";
            foreach (char letter in country)
            {
                lowLtr += char.ToLower(letter);
            }
            return lowLtr;
        }
        [HttpPost] 
        public IActionResult CreateState(string State,string countryId)
        {
            try
            {
                int id = Convert.ToInt32(countryId);
                string state = StateStoreOrder(State);
                string message = "SomeThing Went Wrong";
                string conStr = _config.GetConnectionString("MyApp")!;
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    //SqlCommand cmd = new SqlCommand($"Insert into Countries1 values('{country}')",conn);
                    SqlCommand cmd = new SqlCommand("InsGenState", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@state", state);
                    cmd.Parameters.AddWithValue("@countryId", id);
                    cmd.Parameters.Add("@result", System.Data.SqlDbType.VarChar, 50).Direction = System.Data.ParameterDirection.Output;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    string res = cmd.Parameters["@result"].Value.ToString()!;
                    if (res == "Success")
                    {
                        message = $"State {state} Added Successfully!";
                    }
                    else
                        message = $"{res} While Adding {state}!";
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
        public IActionResult DeleteState(string stateId)
        {
            try
            {
                int id = Convert.ToInt32(stateId);
                //string district = DistrictStoreOrder(District);
                string message = "SomeThing Went Wrong";
                string conStr = _config.GetConnectionString("MyApp")!;
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    //SqlCommand cmd = new SqlCommand($"Insert into Countries1 values('{country}')",conn);
                    SqlCommand cmd = new SqlCommand("DeleteState", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@stateId", id);
                    cmd.Parameters.Add("@result", System.Data.SqlDbType.VarChar, 50).Direction = System.Data.ParameterDirection.Output;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    string res = cmd.Parameters["@result"].Value.ToString()!;
                    if (res == "Success")
                    {
                        message = $"state deleted Successfully!";
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
