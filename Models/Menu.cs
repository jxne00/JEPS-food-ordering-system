using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FYP.Models
{
    public class Menu
    {
        public int Menu_itemid { get; set; }

        [Required(ErrorMessage = "Please enter Item Name")]
        [StringLength(45, ErrorMessage = "Item Name must be less than 45 characters")]
        public string Menu_itemDescription { get; set; }
        
        [Required(ErrorMessage = "Please enter Price")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Price must be more than 0 dollars")]
        public double Menu_itemPrice { get; set; }

        [Required(ErrorMessage = "Please choose a Photo")]
        public IFormFile Photo { get; set; }

        public string Menu_itemPicture { get; set; }

        public int Menu_item_category_id { get; set; }

        public string Menu_item_category_description { get; set; }

        public int Order_detail_id { get; set; }

        //public List<SetMeal> SetMeals { get; set; }

        public virtual ICollection<SetMeal> SetMeals { get; set; }
    }
}