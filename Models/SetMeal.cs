using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace FYP.Models
{
    public class SetMeal
    {
        public int Set_meal_id { get; set; }

        [Required(ErrorMessage = "Please enter Meal Name")]
        [StringLength(45, ErrorMessage = "Meal Name must be less than 45 characters")]
        public string Set_meal_description { get; set; }
        
        [Required(ErrorMessage = "Please enter Price")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Price must be more than 0 dollars")]
        public double Set_meal_price { get; set; }

        [Required(ErrorMessage = "Please choose a Photo")]
        public IFormFile Photo { get; set; }

        public string Set_meal_picture { get; set; }

        [Required(ErrorMessage = "Please enter Menu Item 1")]
        //[Remote(action: "VerifyMenuItem", controller: "Menu")]
        //[MenuItem(ErrorMessage = "Invalid Menu Item")]
        public int Menu_itemid1 { get; set; }
        
        [Required(ErrorMessage = "Please enter Menu Item 2")]
        //[Remote(action: "VerifyMenuItem", controller: "Menu")]
        //[MenuItem(ErrorMessage = "Invalid Menu Item")]
        public int Menu_itemid2 { get; set; }

        public string Menu_itemDescription { get; set; }

        //public List<Menu> Menus { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}