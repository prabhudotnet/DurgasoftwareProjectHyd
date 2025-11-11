using checkboxDynamic.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;

namespace checkboxDynamic.Controllers
{
    public class CountryController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DbHelper _dbHelper;



        public CountryController(IConfiguration configuration, DbHelper dbHelper)
        {
            _config = configuration;
            _dbHelper = dbHelper;

        }

        public IActionResult Index()
        {

            return View("Index");
        }
       

       
        public string CountryStoreOrder(string country)
        {
            string lowLtr = "";
            foreach (char letter in country)
            {
                lowLtr += char.ToLower(letter);
            }
            return lowLtr;
        }

        [HttpPost]
        public IActionResult CreateCountry(string Country)
        {
            try
            {
                string country = CountryStoreOrder(Country);
                string message = "SomeThing Went Wrong";
                string conStr = _config.GetConnectionString("MyApp")!;
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    //SqlCommand cmd = new SqlCommand($"Insert into Countries1 values('{country}')",conn);
                    SqlCommand cmd = new SqlCommand("InsGenCountries", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@country", country);
                    cmd.Parameters.Add("@result", System.Data.SqlDbType.VarChar, 50).Direction = System.Data.ParameterDirection.Output;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    string res = cmd.Parameters["@result"].Value.ToString()!;
                    if (res == "Success")
                    {
                        message = $"Country {country} Added Successfully!";
                    }
                    else
                        message = $"{res} While Adding {country}!";
                }
                TempData["Msg"] = message;
            }
            catch (Exception ex)
            {
                TempData["Msg"] = ex.Message;
                return RedirectToAction("AddInterest","Admin");

            }
            return RedirectToAction("AddInterest", "Admin");
        }
        
        [HttpPost]
        public IActionResult DeleteCountry(string countryVal)
        {
            try
            {
                int id = Convert.ToInt32(countryVal);
                //string district = DistrictStoreOrder(District);
                string message = "SomeThing Went Wrong";
                string conStr = _config.GetConnectionString("MyApp")!;
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    //SqlCommand cmd = new SqlCommand($"Insert into Countries1 values('{country}')",conn);
                    SqlCommand cmd = new SqlCommand("DeleteCountry", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@countryId", id);
                    cmd.Parameters.Add("@result", System.Data.SqlDbType.VarChar, 50).Direction = System.Data.ParameterDirection.Output;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    string res = cmd.Parameters["@result"].Value.ToString()!;
                    if (res == "Success")
                    {
                        message = $"Country deleted Successfully!";
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
