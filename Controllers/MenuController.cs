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
    public class MenuController : Controller
    {
        //ALL USERS
        #region DETAILS OF MENU ITEM
        [AllowAnonymous]
        public IActionResult MenuDetails(int id)
        {
            string sql = @"SELECT * FROM Menu_item WHERE Menu_itemid={0}";

            string select = String.Format(sql, id);
            List<Menu> lstMenu = DBUtl.GetList<Menu>(select);
            if (lstMenu.Count == 1)
            {
                Menu menu = lstMenu[0];
                return View("MenuDetails", menu);
            }
            else
            {
                TempData["Message"] = "Menu Item not found";
                TempData["MsgType"] = "warning";
                return RedirectToAction("ListMenu");
            }
        }
        #endregion

        #region DETAILS OF SET MEAL
        [AllowAnonymous]
        public IActionResult SetDetails(int id)
        {
            string sql =
               @"SELECT *
                FROM Set_meal
               WHERE Set_meal_id={0}";

            string select = String.Format(sql, id);
            List<SetMeal> lstMeal = DBUtl.GetList<SetMeal>(select);
            if (lstMeal.Count == 1)
            {
                SetMeal setmeal = lstMeal[0];
                return View("SetDetails", setmeal);
            }
            else
            {
                //TempData["Message"] = "Set Meal not found";
                TempData["MsgType"] = "warning";
                return RedirectToAction("ListMeal");
            }
        }
        #endregion


        //VISITOR AND CUSTOMER
        #region DISPLAY ALL MENU
        [AllowAnonymous]
        public IActionResult SetMeals()
        {
            string select = String.Format(@"SELECT * FROM Set_meal");
            List<SetMeal> list = DBUtl.GetList<SetMeal>(select);
            return View("SetMeals", list);
        }

        [AllowAnonymous]
        public IActionResult AlaCarte()
        {
            string select = String.Format(@"SELECT * FROM Menu_item WHERE Menu_item_category_id=20000001");
            List<Menu> list = DBUtl.GetList<Menu>(select);
            return View("AlaCarte", list);
        }

        [AllowAnonymous]
        public IActionResult Sides()
        {
            string select = String.Format(@"SELECT * FROM Menu_item WHERE Menu_item_category_id=20000002");
            List<Menu> list = DBUtl.GetList<Menu>(select);
            return View("Sides", list);
        }

        [AllowAnonymous]
        public IActionResult Desserts()
        {
            string select = String.Format(@"SELECT * FROM Menu_item WHERE Menu_item_category_id=20000003");
            List<Menu> list = DBUtl.GetList<Menu>(select);
            return View("Desserts", list);
        }

        [AllowAnonymous]
        public IActionResult Drinks()
        {
            string select = String.Format(@"SELECT * FROM Menu_item WHERE Menu_item_category_id=20000004");
            List<Menu> list = DBUtl.GetList<Menu>(select);
            return View("Drinks", list);
        }
        #endregion


        //STAFF AND ADMIN
        #region LIST OF MENU ITEMS
        [Authorize(Roles = "A, S")]
        public IActionResult ListMenu()
        {
            List<Menu> menu = DBUtl.GetList<Menu>(
                  @"SELECT * FROM Menu_item, Menu_item_category WHERE Menu_item.Menu_item_category_id = Menu_item_category.Menu_item_category_id");
            return View(menu);
        }
        #endregion

        #region LIST OF SET MEALS
        [Authorize(Roles = "A, S")]
        public IActionResult ListMeal()
        {
            List<SetMeal> setmeal = DBUtl.GetList<SetMeal>(@"SELECT * FROM Set_meal");
            return View(setmeal);
        }
        #endregion


        //ADMIN
        #region CREATE MENU ITEM
        [Authorize(Roles = "A")]
        public IActionResult CreateMenu()
        {
            ViewData["Category"] = GetListCategory();
            return View();
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult CreateMenu(Menu menu, IFormFile photo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Category"] = GetListCategory();
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("CreateMenu");
            }
            else
            {
                string picfilename = DoPhotoUpload(menu.Photo);

                string insert =
                   @"INSERT INTO Menu_item(Menu_itemDescription, Menu_itemPrice, Menu_itemPicture, Menu_item_category_id)
                                VALUES('{0}',{1},'{2}',{3})";

                int result = DBUtl.ExecSQL(insert, menu.Menu_itemDescription, menu.Menu_itemPrice, picfilename, menu.Menu_item_category_id);

                if (result == 1)
                {
                    TempData["Message"] = "Menu Item Added Successfully";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("ListMenu");
            }
        }
        #endregion

        #region UPDATE MENU ITEM
        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult EditMenu(int id)
        {
            string menuSql = @"SELECT Menu_itemid, Menu_itemDescription, Menu_itemPrice, Menu_itemPicture, Menu_item_category.Menu_item_category_id FROM Menu_item, Menu_item_category WHERE Menu_item.Menu_item_category_id = Menu_item_category.Menu_item_category_id AND Menu_item.Menu_itemid={0}";
            List<Menu> lstMenu = DBUtl.GetList<Menu>(menuSql, id);

            // If the record is found, pass the model to the View
            if (lstMenu.Count == 1)
            {
                ViewData["Category"] = GetListCategory();
                return View(lstMenu[0]);
            }
            else
            {
                TempData["Message"] = "Menu Item cannot be updated";
                TempData["MsgType"] = "warning";
                return RedirectToAction("ListMenu");
            }
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult EditMenu(Menu menu)
        {
            ModelState.Remove("Photo");
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View("EditMenu");
            }
            else
            {
                string update = @"UPDATE Menu_item  
                              SET Menu_itemDescription='{1}', Menu_itemPrice={2}, Menu_item_category_id={3}
                            WHERE Menu_itemid={0}";
                int res = DBUtl.ExecSQL(update, menu.Menu_itemid, menu.Menu_itemDescription.EscQuote(), menu.Menu_itemPrice, menu.Menu_item_category_id);
                if (res == 1)
                {
                    TempData["Message"] = "Menu Item Updated Successfully";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("ListMenu");
            }
        }
        #endregion

        #region DELETE MENU ITEM
        [Authorize(Roles = "A")]
        public IActionResult DeleteMenu(int id)
        {
            string select = @"SELECT * FROM Menu_item WHERE Menu_itemid={0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Menu Item no longer exists";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string photoFile = ds.Rows[0]["Menu_itemPicture"].ToString();
                string fullpath = Path.Combine(_env.WebRootPath, "images/menu/" + photoFile);

                // Delete the Photo from the Web Server
                System.IO.File.Delete(fullpath);

                string delete = "DELETE FROM Menu_item WHERE Menu_itemid={0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Menu Item Deleted Successfully";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("ListMenu");
        }
        #endregion

        #region CREATE SET MEAL
        [Authorize(Roles = "A")]
        public IActionResult CreateMeal()
        {
            ViewData["Menu"] = GetListMenu();
            return View();
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult CreateMeal(SetMeal setmeal, IFormFile photo)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Menu"] = GetListMenu();
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("CreateMeal");
            }
            else
            {
                string picfilename = DoPhotoUpload(setmeal.Photo);

                string sql = @"INSERT INTO Set_meal(Set_meal_description, Menu_itemid1, Menu_itemid2, Set_meal_price, Set_meal_picture) VALUES('{0}',{1},{2},{3},'{4}')";

                string insert = String.Format(sql, setmeal.Set_meal_description, setmeal.Menu_itemid1, setmeal.Menu_itemid2, setmeal.Set_meal_price, picfilename);

                if (DBUtl.ExecSQL(insert) == 1)
                {
                    TempData["Message"] = "Set Meal Added Successfully";
                    TempData["MsgType"] = "success";
                    return RedirectToAction("ListMeal");
                }
                else
                {
                    ViewData["Message"] = DBUtl.DB_Message;
                    ViewData["MsgType"] = "danger";
                    return View("CreateMeal");
                }
            }
        }
        #endregion

        #region UPDATE SET MEAL
        [Authorize(Roles = "A")]
        [HttpGet]
        public IActionResult EditMeal(int id)
        {
            string mealSql = @"SELECT * FROM Set_meal WHERE Set_meal_id={0}";
            List<SetMeal> lstMeal = DBUtl.GetList<SetMeal>(mealSql, id);

            // If the record is found, pass the model to the View
            if (lstMeal.Count == 1)
            {
                return View(lstMeal[0]);
            }
            else
            {
                TempData["Message"] = "Set Meal cannot be updated";
                TempData["MsgType"] = "warning";
                return RedirectToAction("ListMeal");
            }
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public IActionResult EditMeal(SetMeal setmeal)
        {
            ModelState.Remove("Photo");
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "danger";
                return View("EditMeal");
            }
            else
            {
                string update = @"UPDATE Set_meal SET Set_meal_description='{1}', Menu_itemid1={2}, Menu_itemid2={3}, Set_meal_price={4} WHERE Set_meal_id={0}";
                int res = DBUtl.ExecSQL(update, setmeal.Set_meal_id, setmeal.Set_meal_description.EscQuote(), setmeal.Menu_itemid1, setmeal.Menu_itemid2, setmeal.Set_meal_price);
                if (res == 1)
                {
                    TempData["Message"] = "Set Meal Updated Successfully";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("ListMeal");
            }
        }
        #endregion

        #region DELETE SET MEAL
        [Authorize(Roles = "A")]
        public IActionResult DeleteMeal(int id)
        {
            string select = @"SELECT * FROM Set_meal WHERE Set_meal_id={0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Set Meal no longer exists";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string photoFile = ds.Rows[0]["Set_meal_picture"].ToString();
                string fullpath = Path.Combine(_env.WebRootPath, "images/menu/" + photoFile);

                // Delete the Photo from the Web Server
                System.IO.File.Delete(fullpath);

                string delete = "DELETE FROM Set_meal WHERE Set_meal_id={0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Set Meal Deleted Successfully";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("ListMeal");
        }
        #endregion



        #region GET LISTS OF ALL INFORMATION FROM THE DATABASE
        private List<Category> GetListCategory()
        {
            string categorySql = @"SELECT Menu_item_category_id, Menu_item_category_description 
                                FROM Menu_item_category";
            List<Category> lstCategory = DBUtl.GetList<Category>(categorySql);
            return lstCategory;
        }

        private List<Menu> GetListMenu()
        {
            string menuSql = @"SELECT Menu_itemid, Menu_itemDescription
                                FROM Menu_item";
            List<Menu> lstMenu = DBUtl.GetList<Menu>(menuSql);
            return lstMenu;
        }
        #endregion
        

        private void UploadFile(IFormFile ufile, string fname)
        {
            string fullpath = Path.Combine(_env.WebRootPath, fname);
            using (var fileStream = new FileStream(fullpath, FileMode.Create))
            {
                ufile.CopyToAsync(fileStream);
            }
        }

        private string DoPhotoUpload(IFormFile photo)
        {
            string fext = Path.GetExtension(photo.FileName);
            string uname = Guid.NewGuid().ToString();
            string fname = uname + fext;
            string fullpath = Path.Combine(_env.WebRootPath, "images/menu/" + fname);
            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
            {
                photo.CopyTo(fs);
            }
            return fname;
        }

        private IHostingEnvironment _env;
        public MenuController(IHostingEnvironment environment)
        {
            _env = environment;
        }

        // To check if the Menu Item exists when creating
        [Authorize(Roles = "A")]
        public IActionResult VerifyMenuItem(string menuItem)
        {
            string select = $"SELECT * FROM Menu_item WHERE Menu_itemid={menuItem}";
            if (DBUtl.GetTable(select).Rows.Count > 0)
            {
                return Json(true);
            }
            return Json($"The Menu Item [{menuItem}] doesn't exist. Please enter a valid Menu Item.");
        }
    }
}