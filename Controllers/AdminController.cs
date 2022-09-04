using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FYP.Models;
using System;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;

namespace FYP.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "A")]
        public IActionResult Users()
        {
            DataTable dt = DBUtl.GetTable("SELECT * FROM Users");
            return View("Users", dt.Rows);
        }


        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult UserRegister()
        {
            return View();
        }


        [Authorize(Roles = "A")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult UserRegister(VisitorRegistration usr)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("Users");
            }

            else
            {
                string insert =
                   @"INSERT INTO Users(UserEmail, User_fullname, User_Password, User_type, Status) VALUES
                 ('{0}', '{1}', HASHBYTES('SHA1', '{2}'), '{3}', 'Active')";


                if (DBUtl.ExecSQL(insert, usr.UserEmail, usr.User_fullname, usr.User_Password, usr.User_type, usr.Status) == 1)
                {
                    TempData["Message"] = "User successfully registered";
                    TempData["MsgType"] = "success";


                }

                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("Users");
            }
        }

        //[Authorize(Roles = "A")]
        //[HttpGet]
        //public IActionResult DeleteUser()
        //{
        //    // Delete Action for User (GET)
        //    return View("DeleteUser");
        //}

        //[Authorize(Roles = "A")]
        //[HttpPost]
        //public IActionResult DeleteUserPost()
        //{
        //    // Delete Action for User (POST)
        //    IFormCollection form = HttpContext.Request.Form;
        //    string UserEmail = form["UserEmail"].ToString().Trim();

        //    if (!UserEmail.IsNormalized())
        //    {
        //        ViewData["Message"] = "User Email must be incorrect";
        //        ViewData["MsgType"] = "warning";
        //        return View("DeleteUser");
        //    }

        //    string sql = @"SELECT * FROM Users WHERE UserEmail='{0}'";
        //    string select = String.Format(sql, UserEmail);
        //    DataTable dt = DBUtl.GetTable(select);
        //    if (dt.Rows.Count == 0)
        //    {
        //        ViewData["Message"] = "User Not Found";
        //        ViewData["MsgType"] = "warning";
        //        return View("DeleteUser");
        //    }

        //    sql = @"DELETE Users WHERE UserEmail='{0}'";
        //    string delete = String.Format(sql, UserEmail);
        //    int count = DBUtl.ExecSQL(delete);
        //    if (count == 1)
        //    {
        //        ViewData["Message"] = "User Deleted";
        //        ViewData["MsgType"] = "success";
        //    }
        //    else
        //    {
        //        ViewData["Message"] = DBUtl.DB_Message;
        //        ViewData["MsgType"] = "danger";
        //    }

        //    return View("DeleteUser");
        //}



        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult UpdateUserInfo(string id)
        {
            string sql = @"SELECT * FROM Users WHERE UserEmail='{0}'";
            List<VisitorRegistration> list = DBUtl.GetList<VisitorRegistration>(sql, id);
            if (list.Count == 1)
            {
                VisitorRegistration usr = list[0];
                return View(usr);
            }
            else
            {
                ViewData["Message"] = "User not found";
                ViewData["MsgType"] = "warning";
                return View("Users");
            }


        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult UpdateUserInfo(VisitorRegistration usr)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View("UpdateUserInfo");
            }
            else
            {
                string update =
                   @"UPDATE Users
                    SET User_fullname='{1}', User_Password=HASHBYTES('SHA1', '{2}'), User_type='{3}', Status='{4}'
                  WHERE UserEmail='{0}'";
                int res = DBUtl.ExecSQL(update, usr.UserEmail, usr.User_fullname, usr.User_Password, usr.User_type, usr.Status);
                if (res == 1)
                {
                    TempData["Message"] = "User details Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";

                }
                return RedirectToAction("Users");
            }
        }



        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult InactivateUser(string id)
        {
            string UserEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string sql = @"SELECT * FROM Users WHERE UserEmail='{0}'";
            List<VisitorRegistration> list = DBUtl.GetList<VisitorRegistration>(sql, id);
            if (list.Count == 1)
            {
                VisitorRegistration pw = list[0];
                return View(pw);
            }
            else
            {
                ViewData["Message"] = "User not found";
                ViewData["MsgType"] = "warning";
                return View("Users");
            }


        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult InactivateUser(VisitorRegistration usr)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View("InactivateUser");
            }
            else
            {
                string Userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                string update =
                   @"UPDATE Users
                    SET User_fullname='{1}', User_Password='Inactive', User_type='{3}', Status='Inactive', Inactive_status ='{5}'
                  WHERE UserEmail='{0}'";
                int res = DBUtl.ExecSQL(update, usr.UserEmail, usr.User_fullname, usr.User_Password, usr.User_type, usr.Status, usr.Inactive_status);
                if (res == 1)
                {
                    TempData["Message"] = "User details Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";

                }
                return RedirectToAction("Users");
            }
        }



        //[Authorize(Roles = "A")]
        //public IActionResult Customer()
        //{
        //    DataTable dt = DBUtl.GetTable("SELECT * FROM Customer");
        //    return View("Customer", dt.Rows);
        //}

        [Authorize(Roles = "A")]
        public IActionResult Customer()
        {
            DataTable dt = DBUtl.GetTable("SELECT C.Customer_id, C.UserEmail, U.User_fullname, C.CustomerNo, C.Date_of_birth, C.Customer_Address FROM Customer C, Users U WHERE C.UserEmail = U.UserEmail AND U.Status = 'Active';; ");
            return View("Customer", dt.Rows);
        }


        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult CustIndex(string id)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string sql = @"SELECT * FROM Customer WHERE UserEmail='{0}'";

            List<CustomerDetails> lstCust = DBUtl.GetList<CustomerDetails>(sql, id);
            if (lstCust.Count == 1)
            {
                CustomerDetails cust = lstCust[0];
                return View(cust);
            }
            else
            {
                TempData["Message"] = "Customer Record does not exist";
                TempData["MsgType"] = "warning";
                return RedirectToAction("CustIndex");
            }
        }

        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult CustUpdate(string id)
        {
            string UserEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string sql = @"SELECT * FROM Customer WHERE UserEmail='{0}'";

            List<CustomerDetails> lstCust = DBUtl.GetList<CustomerDetails>(sql, id);
            if (lstCust.Count == 1)
            {
                CustomerDetails cust = lstCust[0];
                return View(cust);
            }
            else
            {
                ViewData["Message"] = "Customer Record does not exist";
                ViewData["MsgType"] = "warning";
                return View("CustUpdate");
            }
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult CustUpdate(CustomerDetails cust)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View(cust);
            }
            else
            {
                string Userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                string sql = @"UPDATE Customer  
                             SET CustomerNo={1}, Date_of_birth='{2:yyyy-MM-dd}', Customer_Address='{3}'
                            WHERE UserEmail='{0}'";


                if (DBUtl.ExecSQL(sql, cust.UserEmail, cust.CustomerNo,
                    cust.Date_of_birth, cust.Customer_Address) == 1)

                {
                    TempData["Message"] = "Personal Information Updated!";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";

                    return RedirectToAction("CustUpdate");
                }

                return RedirectToAction("Customer");
            }
        }

        //[Authorize(Roles = "A")]
        //[HttpGet]
        //public IActionResult DeleteCustomer()
        //{
        //    View for cust delete
        //    return View("DeleteCustomer");
        //}

        //[Authorize(Roles = "A")]
        //[HttpPost]
        //public IActionResult DeleteCustomerPost()
        //{
        //    cust delete action
        //   IFormCollection form = HttpContext.Request.Form;
        //    string Userid = form["Userid"].ToString().Trim();

        //    if (!Userid.IsNormalized())
        //    {
        //        ViewData["Message"] = "User ID must be an integer";
        //        ViewData["MsgType"] = "warning";
        //        return View("DeleteCustomer");
        //    }

        //    string sql = @"SELECT * FROM Customer WHERE Userid='{0}'";
        //    string select = String.Format(sql, Userid);
        //    DataTable dt = DBUtl.GetTable(select);
        //    if (dt.Rows.Count == 0)
        //    {
        //        ViewData["Message"] = "Customer Not Found";
        //        ViewData["MsgType"] = "warning";
        //        return View("DeleteCustomer");
        //    }

        //    sql = @"DELETE FROM Customer WHERE Userid='{0}'";
        //    string delete = String.Format(sql, Userid);
        //    int count = DBUtl.ExecSQL(delete);
        //    if (count == 1)
        //    {
        //        ViewData["Message"] = "Customer Deleted";
        //        ViewData["MsgType"] = "success";
        //    }
        //    else
        //    {
        //        ViewData["Message"] = DBUtl.DB_Message;
        //        ViewData["MsgType"] = "danger";
        //    }

        //    return View("DeleteCustomer");
        //}


        [Authorize(Roles = "A")]
        public IActionResult Birthday(string id)
        {
            //DataTable dt = DBUtl.GetTable("SELECT * FROM Customer");
            string sql = @"SELECT C.Customer_id, U.User_fullname, C.Birthday_discount_used  
                FROM Customer C, Users U  
                WHERE C.UserEmail = U.UserEmail";
            DataTable dt = DBUtl.GetTable(sql, id);
            return View("Birthday", dt.Rows);
        }

    }




    public class Users
    {
        public object[] Userid { get; internal set; }
        public object User_fullname { get; internal set; }
        public object User_Password { get; internal set; }
        public object User_type { get; internal set; }
    }
}