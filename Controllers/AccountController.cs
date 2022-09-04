using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;
using FYP.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace FYP.Controllers
{
    public class AccountController : Controller
    {

        private const string LOGIN_SQL =
           @"SELECT * FROM Users 
            WHERE UserEmail = '{0}' 
              AND User_Password = HASHBYTES('SHA1', '{1}')";

        //setting last login date whenever user logins
        private const string LASTLOGIN_SQL =
           @"UPDATE Users SET LastLogin=GETDATE() WHERE UserEmail='{0}'";

        private const string STOPLOGIN = @"SELECT * FROM Users 
            WHERE UserEmail = '{0}'
              AND StopLogin = 'Y'";

        //User role & user's name field in database 
        private const string ROLE_COL = "User_type";
        private const string NAME_COL = "User_fullname";

        //Page displayed after login
        private const string REDIRECT_CNTR = "Account";
        private const string REDIRECT_ACTN = "AfterLogin";

        private const string LOGIN_CNTR = "Account";
        private const string LOGIN_ACTN = "Login";

        private const string LOGIN_VIEW = "UserLogin";
       

        #region "About us, contact us, afterlogin,stoplogin, forbidden page"
        // ABOUT PAGE
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        //CONTACT US PAGE
        [AllowAnonymous]
        public IActionResult ContactUs()
        {
            return View();
        }

        // LOGIN DISABLED PAGE
        [AllowAnonymous]
        public IActionResult StopLogin()
        {
            return View();
        }

        [Authorize]
        public IActionResult AfterLogin()
        {
            return View();
        }        
        
        // Displays FORBIDDEN page if user does not have permission to view that URL (anyone)
        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }
        #endregion


        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(LOGIN_VIEW);

        }

        //LOGIN PAGE
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(UserLogin user)
        {
            if (!AuthenticateUser(user.UserEmail, user.User_Password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect Email or Password Entered or your account has been deleted. Please try again.";
                ViewData["MsgType"] = "danger";
                return View(LOGIN_VIEW);

            }

            else if (!StopLogin(user.UserEmail, user.StopLogin))
            {
                return View("StopLogin");
            }

            else
            {
                //REMEMBER ME CHECKBOX
                HttpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   principal,
                   new AuthenticationProperties
                   {
                       IsPersistent = user.RememberMe // 
                   });


                // UPDATING LASTLOGIN OF USER
                DBUtl.ExecSQL(LASTLOGIN_SQL, user.UserEmail);

                if (TempData["returnUrl"] != null)
                {
                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                }


                //    string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //    string stop = String.Format(@"SELECT StopLogin FROM Users 
                //WHERE UserEmail = '{0}' AND User_type = 'C'", userid);

                //    if (stop.Equals("Y"))
                //    {
                //        return View("StopLogin");
                //    }

                return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
            }
        }

        // USER LOGOUT
        [Authorize]
        public IActionResult Logoff(string returnUrl = null)
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(LOGIN_ACTN, LOGIN_CNTR);
        }
       
        // To check if the Email and Password is valid during login
        private bool AuthenticateUser(string uid, string pw, out ClaimsPrincipal principal)
        {
            principal = null;

            DataTable ds = DBUtl.GetTable(LOGIN_SQL, uid, pw);
            if (ds.Rows.Count == 1)
            {
                principal =
                   new ClaimsPrincipal(
                      new ClaimsIdentity(
                         new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, uid),
                        new Claim(ClaimTypes.Name, ds.Rows[0][NAME_COL].ToString()),
                        new Claim(ClaimTypes.Role, ds.Rows[0][ROLE_COL].ToString())
                         }, "Basic"
                      )
                   );
                return true;
            }
            return false;
        }

        //To check if stoplogin is allowed
        private bool StopLogin(string uid,string stop)
        {
            DataTable ds = DBUtl.GetTable(STOPLOGIN, uid, stop);
            if (ds.Rows.Count == 1)
            {
               return false;
            }
            return true;
        }



        // REGISTER AS CUSTOMER (Information for Users table in database)
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View("CustRegister");
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(VisitorRegistration usr)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("CustRegister");
            }

            else
            {
                string insert =
                   @"INSERT INTO Users (UserEmail, User_fullname, User_Password, User_type, Status, Inactive_status) VALUES
                 ('{0}', '{1}', HASHBYTES('SHA1', '{2}'),'C','Active','N')";

                // User_type set to 'C' default as this form is only to register customers

                if (DBUtl.ExecSQL(insert, usr.UserEmail, usr.User_fullname, usr.User_Password, usr.User_type,usr.Status,usr.Inactive_status) == 1)
                {
                    ViewData["Message"] = "Please provide your personal details";
                    ViewData["MsgType"] = "primary";

                    // Redirect to customer details page if successful  
                    return View("CustDetails");
                }

                else
                {
                    ViewData["Message"] = DBUtl.DB_Message;
                    ViewData["MsgType"] = "danger";

                    return View("CustRegister");
                }
            }
        }

        // Customer to enter personal details to insert into Customer table
        public IActionResult CustDetails()
        {
            return View("CustDetails");
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CustDetails(CustomerDetails details)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("CustDetails");
            }

            else
            {
                string insert =
                   @"INSERT INTO Customer(CustomerNo, Date_of_birth, Customer_Address,UserEmail) VALUES
                 ({0}, '{1:yyyy-MM-dd}','{2}','{3}')";


                if (DBUtl.ExecSQL(insert, details.CustomerNo, details.Date_of_birth, details.Customer_Address, details.UserEmail) == 1)
                {
                    //Success message
                    string template = $@"[{details.UserEmail}] Successfully registered! Enter password to login.";

                    ViewData["Message"] = template;
                    ViewData["MsgType"] = "success";
                }

                else
                {
                    ViewData["Message"] = DBUtl.DB_Message;
                    ViewData["MsgType"] = "danger";
                }
                //Directed to login page for customer to login (userid automatically filled)   
                return View("UserLogin");
            }
        }
      
        // To check if the UserEmail is already in use during register 
        [AllowAnonymous]
        public IActionResult VerifyUserEmail(string userEmail)
        {
            string select = $"SELECT * FROM Users WHERE UserEmail='{userEmail}'";
            if (DBUtl.GetTable(select).Rows.Count > 0)
            {
                return Json($"The Email [{userEmail}] is already in use. Please enter a different Email.");
            }
            return Json(true);
        }


        [AllowAnonymous]
        public IActionResult CheckDate(DateTime Date_of_birth)
        {
            if (Date_of_birth > DateTime.Today)
            {
                return Json($"Birthdate cannot be a future date");
            }
            return Json(true);
        }


        [Authorize(Roles = "S")]
        public IActionResult UpdateStopLogin()
        {
            return View("UpdateStopLogin");
        }


        [Authorize(Roles = "S")]
        [HttpPost]
        public IActionResult UpdateStopLoginPost(VisitorRegistration cust)
        {
            IFormCollection form = HttpContext.Request.Form;
            string stoplogin = form["StopLogin"].ToString().Trim();

            string sql = @"SELECT StopLogin
                            FROM Users
                            WHERE User_type = 'C'";

            string select = String.Format(sql);
            DataTable dt = DBUtl.GetTable(select);
            if (dt.Rows.Count == 0)
            {
                ViewData["Mesaage"] = "Records not found";
                ViewData["MsgType"] = "warning";
                return View("UpdateStopLogin");
            }

            sql = @"UPDATE Users 
                    SET StopLogin = 'Y'
                    WHERE User_type = 'C'";
            string update = String.Format(sql, cust.StopLogin);

            if (DBUtl.ExecSQL(sql, cust.StopLogin) == 1)
            {
                ViewData["Message"] = "Online orders stopped!";
                ViewData["MsgType"] = "success";

                return View("UpdateStopLogin");
            }
            else
            {
                ViewData["Message"] = DBUtl.DB_Message;
                ViewData["MsgType"] = "danger";

                return View("UpdateStopLogin");
            }

        }

        [Authorize(Roles = "S")]
        public IActionResult ContinueLogin()
        {
            return View("ContinueLogin");
        }


        [Authorize(Roles = "S")]
        [HttpPost]
        public IActionResult ContinueLoginPost(VisitorRegistration cust)
        {
            IFormCollection form = HttpContext.Request.Form;
            string stoplogin = form["StopLogin"].ToString().Trim();

            string sql = @"SELECT StopLogin
                            FROM Users
                            WHERE User_type = 'C'";

            string select = String.Format(sql);
            DataTable dt = DBUtl.GetTable(select);
            if (dt.Rows.Count == 0)
            {
                ViewData["Mesaage"] = "Records not found";
                ViewData["MsgType"] = "warning";
                return View("ContinueLogin");
            }

            sql = @"UPDATE Users 
                    SET StopLogin = NULL
                    WHERE User_type = 'C'";
            string update = String.Format(sql, cust.StopLogin);

            if (DBUtl.ExecSQL(sql, cust.StopLogin) == 1)
            {
                ViewData["Message"] = "Online orders resumed!";
                ViewData["MsgType"] = "success";

                return View("ContinueLogin");
            }
            else
            {
                ViewData["Message"] = DBUtl.DB_Message;
                ViewData["MsgType"] = "danger";

                return View("ContinueLogin");
            }

        }

        #region "Send Email"
        [Authorize(Roles = "A")]
        public IActionResult SendBirthdayEmail()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "A")]
        public IActionResult SendBirthdayEmail(IFormCollection form)
        {
            string custname = form["User_fullname"].ToString().Trim();
            string email = form["UserEmail"].ToString().Trim();
            string subject = form["Subject"].ToString().Trim();

            string template = @"Hi {0},
                     <p>Your redemption code for birthday discount is <b>{1}</b>. Please use it before the end of the month.</p>
                    Food Ordering System,                    
                    Admin";

            string birthdaydiscount = Guid.NewGuid().ToString().Substring(0, 12);

            string body = String.Format(template, custname, birthdaydiscount);

            string result;

            if (EmailUtl.SendEmail(email, subject, body, out result))
            {
                ViewData["Message"] = "Email Successfully Sent";
                ViewData["MsgType"] = "success";
            }
            else
            {
                ViewData["Message"] = result;
                ViewData["MsgType"] = "warning";
            }

            return View();
        }

        #endregion
    }
}