using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;
using FYP.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FYP.Controllers
{
    public class PromoSampleController : Controller
    {
        [Authorize(Roles = "A")]
        public IActionResult Sample()
        {
            string select = String.Format(@"Select * From Promotion_discount WHERE Promotion_end_date > getdate();");
            List<Promotion> list = DBUtl.GetList<Promotion>(select);
            return View("Sample", list);
        }

        [Authorize(Roles = "C")]
        public IActionResult CustPromo()
        {
            string select = String.Format(@"Select * From Promotion_discount WHERE Promotion_end_date > getdate();");
            List<Promotion> list = DBUtl.GetList<Promotion>(select);
            return View("CustPromo", list);
        }


        //[Authorize(Roles = "A")]
        //[HttpGet]
        //public IActionResult SampleAdd()
        //{
        //    return View();
        //}


        //[Authorize(Roles = "A")]
        //[HttpPost]
        //public IActionResult SampleAdd(Promotion promo)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewData["Message"] = "Invalid Input";
        //        ViewData["MsgType"] = "warning";
        //        return View("Sample");
        //    }

        //    else
        //    {
        //        string insert =
        //                @"INSERT INTO Promotion_discount(Promotion_discount_Description, Promotion_percentage, Promotion_start_date, Promotion_end_date) VALUES
        //         ('{0}', {1}, '{2:yyyy-MM-dd}', '{3:yyyy-MM-dd}')";


        //        if (DBUtl.ExecSQL(insert, promo.Promotion_discount_Description, promo.Promotion_percentage, promo.Promotion_start_date, promo.Promotion_end_date) == 1)
        //        {
        //            TempData["Message"] = "Discount successfully created";
        //            TempData["MsgType"] = "success";
        //        }

        //        else
        //        {
        //            TempData["Message"] = DBUtl.DB_Message;
        //            TempData["MsgType"] = "danger";

        //            return RedirectToAction("SampleAdd");

        //        }
        //        return RedirectToAction("Sample");
        //    }
        //}

        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult AddPromo()
        {
            return View();
        }


        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult AddPromo(Promotion promo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("AddPromo");
            }

            else
            {
                string insert =
                        @"INSERT INTO Promotion_discount(Promotion_discount_Description, Promotion_percentage, Promotion_start_date, Promotion_end_date) VALUES
                 ('{0}', 10, '{2:yyyy-MM-dd}', '{3:yyyy-MM-dd}')";


                if (DBUtl.ExecSQL(insert, promo.Promotion_discount_Description, promo.Promotion_percentage, promo.Promotion_start_date, promo.Promotion_end_date) == 1)
                {
                    TempData["Message"] = "Discount successfully created";
                    TempData["MsgType"] = "success";
                }

                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";

                    return RedirectToAction("AddPromo");

                }
                return RedirectToAction("Sample");
            }
        }


        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult SampleUpdate(string id)
        {
            string Promotion_discount_id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string sql = @"SELECT * FROM Promotion_discount  WHERE Promotion_discount_id='{0}'";
            List<Promotion> list = DBUtl.GetList<Promotion>(sql, id);
            if (list.Count == 1)
            {
                Promotion promo = list[0];
                return View(promo);
            }
            else
            {
                ViewData["Message"] = "User not found";
                ViewData["MsgType"] = "warning";
                return View("Sample");
            }


        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult SampleUpdate(Promotion promo)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View("SampleUpdate");
            }
            else
            {
                string Promotion_discount_id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                string update =
                   @"UPDATE Promotion_discount
                    SET Promotion_discount_Description='{1}', Promotion_percentage= '{2}', Promotion_start_date='{3:yyyy-MM-dd}', Promotion_end_date='{4:yyyy-MM-dd}'
                  WHERE Promotion_discount_id= {0}";
                int res = DBUtl.ExecSQL(update, promo.Promotion_discount_id, promo.Promotion_discount_Description, promo.Promotion_percentage, promo.Promotion_start_date, promo.Promotion_end_date);
                if (res == 1)
                {
                    TempData["Message"] = "Discount details Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";

                }
                return RedirectToAction("Sample");
            }
        }

    }
}