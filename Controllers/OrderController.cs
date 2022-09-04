using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;
using FYP.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace FYP.Controllers
{
    public class OrderController : Controller
    {
        //Customer to display Order Timing page 
        [Authorize(Roles = "C")]
        public IActionResult AdvanceOrder()
        {
            return View();
        }
        //{
        //    string sql = @"SELECT Customer_Address FROM Customer where Customer_id = '{0}'";
        //    List<CustomerDetails> lstord = DBUtl.GetList<CustomerDetails>(sql, id);

        //    if (lstord.Count == 1)
        //    {
        //        CustomerDetails customerDetails = lstord[0];
        //        return View(customerDetails);
        //    }
        //    else
        //    {
        //        ViewData["Message"] = "Customer Address does not exist";
        //        ViewData["MsgType"] = "warning";
        //        return View("AdvanceOrder");
        //    }
        //}


        //Decide OrderNow or AdvanceOrder
        [HttpPost]
        [Authorize(Roles = "C")]
        public IActionResult AdvanceOrder(Orders orders)
        {

            {
                string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                string insert =
                    @"INSERT INTO Orders(Order_Date, Order_Time, Order_Status, Order_price, UserEmail)
                           VALUES('{0:yyyy-MM-dd}', '{1}', 'Pending' , 0.00, '{4}')";
                /*int result = DBUtl.ExecSQL(insert, orders.Order_Date, orders.Order_Time, orders.Order_Status, orders.Order_price, orders.UserEmail); *///safe

                if (DBUtl.ExecSQL(insert, orders.Order_Date, orders.Order_Time, orders.Order_Status, orders.Order_price, userid) == 1)
                {

                    TempData["Message"] = "You may start ordering!";
                    TempData["MsgType"] = "success";
                    return RedirectToAction("SetMeals", "Menu");
                }
                else
                {
                    ViewData["Message"] = DBUtl.DB_Message;
                    ViewData["MsgType"] = "danger";
                    return View("AdvanceOrder");
                }
            }
        }

        //Customer to display place order page for menu item
        [Authorize(Roles = "C")]
        public IActionResult CreateMenuitem(int id)
        {
            string sql = @"SELECT * FROM Menu_item
            WHERE Menu_itemid = {0}";
            List<Orderdetail> lstItem = DBUtl.GetList<Orderdetail>(sql, id);
            if (lstItem.Count == 1)
            {
                Orderdetail orderdetail = lstItem[0];
                return View(orderdetail);
            }
            else
            {
                ViewData["Message"] = "Menu does not exist";
                ViewData["MsgType"] = "warning";
                return View("CreateMenuitem");
            }
        }

        //Specific authorization to place order for menu item
        [HttpPost]
        [Authorize(Roles = "C")]
        public IActionResult CreateMenuitem(Orderdetail orderdetail)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("CreateMenuitem");
            }
            else
            {
                string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                string insert =
                    @"INSERT INTO Order_detail(Menu_itemid,Quantity, Is_set_meal, Order_id)
                           VALUES({1},'N', 10 
                                    WHERE Menu_itemid = {0})";
                //int result = DBUtl.ExecSQL(insert, orderdetail.Quantity, orderdetail.Order_item_number, orderdetail.Is_set_meal, orderdetail.Order_id, orderdetail.Menu_itemid, userid); //safe

                if (DBUtl.ExecSQL(insert, orderdetail.Menu_itemid, orderdetail.Quantity, orderdetail.Order_item_number, orderdetail.Is_set_meal, orderdetail.Order_id) == 1)
                {

                    TempData["Message"] = "Order added to cart successfully!";
                    TempData["MsgType"] = "success";
                    return RedirectToAction("SetMeals", "Menu");
                }
                else
                {
                    ViewData["Message"] = DBUtl.DB_Message;
                    ViewData["MsgType"] = "danger";
                    return View("CreateMenuitem");
                }
            }
        }

        //Customer to display place order page for set meal
        [Authorize(Roles = "C")]
        public IActionResult CreateSetmeal(int id)
        {
            string sql = @"SELECT * FROM Set_meal
            WHERE Set_meal.Set_meal_id = {0}";
            List<Orderdetail> lstItem = DBUtl.GetList<Orderdetail>(sql, id);
            if (lstItem.Count == 1)
            {
                Orderdetail orderdetail = lstItem[0];
                return View(orderdetail);
            }
            else
            {
                ViewData["Message"] = "Menu does not exist";
                ViewData["MsgType"] = "warning";
                return View("CreateSetmeal");
            }

        }

        //Specific authorization to place order for set meal
        [HttpPost]
        [Authorize(Roles = "C")]
        public IActionResult CreateSetmeal(Orderdetail orderdetail)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("CreateSetmeal");
            }
            else
            {
                string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                string insert =
                    @"INSERT INTO Order_detail(Quantity, Is_set_meal, Order_id, Set_meal_id)
                           VALUES({0},'Y', 10, {4})";
                //int result = DBUtl.ExecSQL(insert, orderdetail.Quantity, orderdetail.Order_item_number, orderdetail.Is_set_meal, orderdetail.Order_id, orderdetail.Set_meal_id, userid); //safe

                if (DBUtl.ExecSQL(insert, orderdetail.Quantity, orderdetail.Order_item_number, orderdetail.Is_set_meal, orderdetail.Order_id, orderdetail.Set_meal_id, userid) == 1)
                {

                    TempData["Message"] = "Order added to cart successfully!";
                    TempData["MsgType"] = "success";
                    return RedirectToAction("SetMeals", "Menu");
                }
                else
                {
                    ViewData["Message"] = DBUtl.DB_Message;
                    ViewData["MsgType"] = "danger";
                    return View("CreateSetmeal");
                }
            }
        }

        [Authorize(Roles = "C")]
        public IActionResult List()
        {
            // Get a list of orders from the database
            List<Orderdetail> orderdetail = DBUtl.GetList<Orderdetail>(
                           @"SELECT Order_detail.Quantity, (Order_detail.Quantity * Orders.Order_price) AS TotalPrice
                            FROM Orders, Order_detail
                            WHERE Orders.Order_id = Order_detail.Order_id 
					        AND Orders.Order_Status = 'Pending'
					        ORDER BY Orders.Order_id");
            return View(orderdetail);
        }

        [Authorize(Roles = "C")]
        public IActionResult EditMenuitem(int id)
        {
            string sql = @"SELECT * FROM Menu_item
            WHERE Menu_itemid = {0}";
            List<Orderdetail> lstItem = DBUtl.GetList<Orderdetail>(sql, id);
            if (lstItem.Count == 1)
            {
                Orderdetail orderdetail = lstItem[0];
                return View(orderdetail);
            }
            else
            {
                ViewData["Message"] = "Menu does not exist";
                ViewData["MsgType"] = "warning";
                return View("List");
            }
        }
        [HttpPost]
        [Authorize(Roles = "C")]
        public IActionResult EditMenuitem(Orderdetail orderdetail)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("EditMenuitem");
            }
            else
            {
                string update =
                    @"UPDATE Order_detail SET Quantity='{1}', Is_set_meal ='{2}', Order_id={3} WHERE Order_detail_id= {0}";
                int res = DBUtl.ExecSQL(update, orderdetail.Order_detail_id, orderdetail.Quantity, orderdetail.Is_set_meal, orderdetail.Order_id);
                if (res == 1)
                {
                    TempData["Message"] = "Quantity Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("List");
            }
        }



        private List<Orders> GetListOrders()
        {
            // Get a list of all orders from the database
            string ordersSql = @"SELECT Order_id, Order_Date, Order_Time, Order_Description, Order_Status, Order_price
                                FROM Orders";
            List<Orders> lstOrders = DBUtl.GetList<Orders>(ordersSql);
            return lstOrders;
        }

        private List<Orderdetail> GetListOrderdetails()
        {
            // Get a list of all orders from the database
            string orderdetailSql = @"SELECT Order_detail_id, Quantity, Order_id, Order_Date, Order_Time, Order_Description,
                                       Order_Status, Order_price
                                FROM Order_detail";
            List<Orderdetail> lstOrders = DBUtl.GetList<Orderdetail>(orderdetailSql);
            return lstOrders;
        }

        //---------------------------------------------------------------


        //Update Order Status
        [Authorize(Roles = "S")]
        public IActionResult OrderStatusIndex()
        {
            string select = @"SELECT O.*, OD.Quantity,(OD.Quantity*O.Order_price) AS TotalPrice 
                FROM Orders O, Order_detail OD  
                WHERE OD.Order_id = O.Order_id";


            DataTable dt = DBUtl.GetTable(select);
            return View("OrderStatusIndex", dt.Rows);
        }

        //Update Order Status
        [Authorize(Roles = "S")]
        public IActionResult UpdateStatus(int id)
        {
            string select = "SELECT * FROM Orders WHERE Order_id = {0}";

            List<Order> list = DBUtl.GetList<Order>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }

            return RedirectToAction("OrderStatusIndex");
        }

        //Update Order Status
        [Authorize(Roles = "S")]
        [HttpPost]
        public IActionResult UpdateStatus(Order ord)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("UpdateStatus");
            }
            else
            {
                string update =
                   @"UPDATE Orders
                    SET Order_Date='{1:yyyy-MM-dd}', Order_Time='{2}',Order_price={3}, Order_Status='{4}', UserEmail= '{5}'
                    WHERE Order_id = {0}";

                int res = DBUtl.ExecSQL(update, ord.Order_id, ord.Order_Date, ord.Order_Time, ord.Order_price, ord.Order_Status, ord.UserEmail);
                if (res == 1)
                {
                    //Success message
                    string template = $@"Order Status for [Order ID: {ord.Order_id}] Successfully updated to [{ord.Order_Status}]!";

                    TempData["Message"] = template;
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("OrderStatusIndex");
            }
        }

        //View Order History (All orders)
        [Authorize(Roles = "S")]
        public IActionResult OrderHistoryIndex()
        {

            string select = @"SELECT O.*, OD.Quantity,(OD.Quantity*O.Order_price) AS TotalPrice 
                FROM Orders O, Order_detail OD  
                WHERE OD.Order_id = O.Order_id";


            DataTable dt = DBUtl.GetTable(select);
            return View("OrderHistoryIndex", dt.Rows);
        }


        //View Orders of the day (only displays orders where date = today)
        [Authorize(Roles = "S")]
        public IActionResult TodayOrder()
        {
            DateTime date = DateTime.Today;

            string select = String.Format(@"SELECT O.*, OD.Quantity,(OD.Quantity*O.Order_price) AS TotalPrice   
                FROM Orders O, Order_detail OD  
                WHERE OD.Order_id = O.Order_id
                AND O.Order_Date = '{0:yyyy-MM-dd}'", date);

            DataTable dt = DBUtl.GetTable(select);
            return View("TodayOrder", dt.Rows);
        }


        // For custome to view own order history
        [Authorize(Roles = "C")]
        [HttpGet]
        public IActionResult OrderHistory(string id)
        {
            string UserEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //string sql = @"SELECT * FROM Orders WHERE UserEmail='{0}'";
            string sql = @"SELECT O.*, OD.Quantity, (OD.Quantity * O.Order_price) AS TotalPrice
                FROM Orders O, Order_detail OD  
                WHERE OD.Order_id = O.Order_id
                AND UserEmail='{0}'";
            DataTable dt = DBUtl.GetTable(sql, UserEmail);
            return View("OrderHistory", dt.Rows);
        }

    }
}
