using ClientserachUtiliy.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClientserachUtiliy.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

      
        public ActionResult validateLogin(Login userLogin)
        {
           
            string Message = "";
            int status=0;
            string url = "";

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["DTM"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("SpUserLogin", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userID", userLogin.UserID);
                        cmd.Parameters.AddWithValue("@Password", userLogin.Password);

                        con.Open();
                        SqlDataReader _sqlDataReader = cmd.ExecuteReader();


                        if (_sqlDataReader.Read())
                        {
                            FormsAuthentication.SetAuthCookie(userLogin.UserID, false);
                            Session["UserID"] = _sqlDataReader[1].ToString();
                            Message = "Login Successfully..";
                            status = 1;
                            url = ConfigurationManager.AppSettings["Fresh"].ToString();
                        
                        }
                        else
                        {

                            Message = "Enter valide Username & Password";
                            url = ConfigurationManager.AppSettings["Login"].ToString();

                        }




                    }
                }
                return Json(new
                {
                    status = status,
                    message = Message,
                    url = url
                });
            }
            catch (Exception ex)
            {
               string massage = ex.Message.ToString();
            }

            return null;
            

            
        }

        public ActionResult Login()
        {
            return View();
        }

       public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        
        [HttpGet]
       public ActionResult SessionExpired()
        {
            return View();
        }
    }
}