using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;


namespace FYP.Models
{
    public class Orderdetail
    {
        public int Order_detail_id { get; set; }

        [Required(ErrorMessage = "Please select Quantity")]
        //[Range(1, 10, ErrorMessage = "Invalid Quantity")]
        public int Quantity { get; set; }

        public int Order_item_number { get; set; }

        public string Is_set_meal { get; set; }

        [ForeignKey("Orders")]
        public int Order_id { get; set; }
        public Orders Orders { get; set; }
        public int Order_price { get; set; }

        public int Menu_itemid { get; set; }
        public string Menu_itemDescription { get; set; }
        public double Menu_itemPrice { get; set; }
        public IFormFile Photo { get; set; }
        public string Menu_itemPicture { get; set; }

        public int Set_meal_id { get; set; }
        public string Set_meal_description { get; set; }
        public double Set_meal_price { get; set; }
        public string Set_meal_picture { get; set; }

    }
}
