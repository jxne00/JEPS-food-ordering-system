using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using FYP.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FYP.Controllers
{
    public class GraphController : Controller
    {

        //Customer with most orders made
        [Authorize(Roles = "A")]
        public IActionResult MostOrderMade()
        {
            string select = @"SELECT UserEmail, COUNT(Order_id) AS Order_id
                                     FROM Orders
                                        WHERE YEAR(Order_Date) = YEAR(getdate())
                                        AND MONTH(Order_Date) = MONTH(getdate())
                                      GROUP BY UserEmail
                                      ORDER BY 2 DESC";

            DataTable dt = DBUtl.GetTable(select);
            return View(dt.Rows);
        }

        //Customer with most spending amount
        [Authorize(Roles = "A")]
        public IActionResult MostSpendings()
        {
            string select = @"SELECT O.UserEmail, SUM(O.Order_price*OD.Quantity) AS Order_price
                                     FROM Orders O, Order_detail OD
WHERE O.Order_id = OD.Order_id
AND YEAR(Order_Date) = YEAR(getdate())
AND MONTH(Order_Date) = MONTH(getdate())
                                      GROUP BY UserEmail
                                      ORDER BY 2 DESC";

            DataTable dt = DBUtl.GetTable(select);
            return View(dt.Rows);
        }

        [Authorize(Roles = "A")]
        public IActionResult Sample()
        {
            return View();
        }

        [Authorize(Roles = "A")]
        public IActionResult SideDish()
        {
            return View();
        }

        [Authorize(Roles = "A")]
        public IActionResult SetMeal()
        {
            return View();
        }

    }
}
