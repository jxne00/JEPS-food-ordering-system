using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FYP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;

namespace FYP.Controllers
{
    public class CustomerController : Controller
    {

        [Authorize(Roles = "C")]
        [HttpGet]
        public IActionResult AllDetails()
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string select = @"SELECT C.*, U.User_fullname, U.User_Password
                    FROM Customer C, Users U 
                    WHERE C.UserEmail = U.UserEmail
                     AND C.UserEmail = '{0}'";

            DataTable dt = DBUtl.GetTable(select, userid);

            if (dt.Rows.Count == 1)
            {
                return View("AllDetails", dt.Rows);
            }
            else
            {
                TempData["Message"] = "Customer Record does not exist";
                TempData["MsgType"] = "warning";
                return RedirectToAction("AllDetails");
            }
        }

        // Display customer personal information (for the logged in account) 
        [Authorize(Roles = "C")]
        public IActionResult CustIndex()
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string sql = @"SELECT * FROM Customer WHERE userEmail = '{0}'";

            List<CustomerDetails> lstCust = DBUtl.GetList<CustomerDetails>(sql, userid);
            if (lstCust.Count == 1)
            {
                CustomerDetails cust = lstCust[0];
                return View("CustIndex", cust);
            }
            else
            {
                TempData["Message"] = "Customer Record does not exist";
                TempData["MsgType"] = "warning";
                return RedirectToAction("CustIndex");
            }
        }

        //Update Personal information
        [Authorize(Roles = "C")]
        public IActionResult CustUpdate(string id)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string sql = @"SELECT * FROM Customer 
                         WHERE userEmail='{0}'";

            List<CustomerDetails> lstCust = DBUtl.GetList<CustomerDetails>(sql, userid);
            if (lstCust.Count == 1)
            {
                CustomerDetails cust = lstCust[0];
                return View(cust);
            }
            else
            {
                ViewData["Message"] = "Customer Record does not exist";
                ViewData["MsgType"] = "warning";
                return View("CustIndex");
            }
        }

        [Authorize(Roles = "C")]
        [HttpPost]
        public IActionResult CustUpdate(CustomerDetails cust)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View("CustIndex", cust);
            }
            else
            {
                string sql = @"UPDATE Customer  
                              SET CustomerNo={1}, Date_of_birth='{2:yyyy-MM-dd}', Customer_Address='{3}'
                            WHERE UserEmail='{0}'";

                if (DBUtl.ExecSQL(sql, cust.UserEmail, cust.CustomerNo,
                    cust.Date_of_birth, cust.Customer_Address) == 1)

                {
                    ViewData["Message"] = "Personal Information Updated!";
                    ViewData["MsgType"] = "success";
                }
                else
                {
                    ViewData["Message"] = DBUtl.DB_Message;
                    ViewData["MsgType"] = "danger";

                    return View("CustUpdate");
                }

                return View("CustIndex");
            }
        }


        // UPDATE FULLNAME AND PASSWORD
        [Authorize(Roles = "C")]
        public IActionResult AccountDetail()
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string sql = @"SELECT User_fullname, User_Password FROM Users WHERE userEmail = '{0}'";


            List<VisitorRegistration> lstCust = DBUtl.GetList<VisitorRegistration>(sql, userid);
            if (lstCust.Count == 1)
            {
                VisitorRegistration cust = lstCust[0];
                return View("AccountDetail", cust);
            }

            else
            {
                TempData["Message"] = "Customer Record does not exist";
                TempData["MsgType"] = "warning";
                return RedirectToAction("AccountDetail");
            }
        }


        [Authorize(Roles = "C")]
        public IActionResult AccountUpdate()
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string sql = @"SELECT * FROM Users 
                         WHERE userEmail='{0}'";

            List<VisitorRegistration> lstCust = DBUtl.GetList<VisitorRegistration>(sql, userid);
            if (lstCust.Count == 1)
            {
                VisitorRegistration cust = lstCust[0];
                return View(cust);
            }
            else
            {
                ViewData["Message"] = "Account Record does not exist";
                ViewData["MsgType"] = "warning";
                return View("AccountDetail");
            }
        }

        [Authorize(Roles = "C")]
        [HttpPost]
        public IActionResult AccountUpdate(VisitorRegistration cust)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid";
                ViewData["MsgType"] = "danger";
                return View("AccountDetail", cust);
            }
            else
            {

                string sql = @"UPDATE Users  
                              SET User_fullname ='{1}', User_Password = '{2}', User_type ='C'
                            WHERE userEmail='{0}'";

                if (DBUtl.ExecSQL(sql, cust.UserEmail, cust.User_fullname, cust.User_Password, cust.User_type) == 1)

                {
                    ViewData["Message"] = "Account Details Updated!";
                    ViewData["MsgType"] = "success";

                    return View("AccountDetail");
                }
                else
                {
                    ViewData["Message"] = DBUtl.DB_Message;
                    ViewData["MsgType"] = "danger";

                    return View("AccountUpdate");
                }


            }

        }

        //List of orders made by specific customer
        [Authorize(Roles = "A,S")]
        public IActionResult CustOrderList(string id)
        {
            //string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string select = String.Format(@"SELECT O.*, OD.Quantity, (OD.Quantity*O.Order_price) AS TotalPrice  
                FROM Orders O, Order_detail OD  
                WHERE OD.Order_id = O.Order_id
                AND O.UserEmail = '{0}'", id);

            DataTable dt = DBUtl.GetTable(select);
            return View("CustOrderList", dt.Rows);
        }

        //[Authorize(Roles = "C")]
        //[HttpPost]
        //public IActionResult StopLogin()
        //{
        //    string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    string sql = String.Format(@"SELECT StopLogin FROM Users 
        //                 WHERE userEmail='{0}'", userid);

        //    if (sql.Equals("NULL"))
        //    {
        //        return View("test");
        //    }

        //    return View("StopLogin");
        //}

    }
}