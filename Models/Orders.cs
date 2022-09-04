using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Orders
    {
        public int Order_id { get; set; }

        [Required(ErrorMessage = "Please select order date")]
        public DateTime Order_Date { get; set; }

        [Required(ErrorMessage = "Please key in 24 hours timing")]
        [RegularExpression("([01][0-9]|2[0-3])[0-5][0-9]", ErrorMessage = "Invalid Order Timing")]
        public DateTime Order_Time { get; set; }

        public string Order_Status { get; set; }

        public double Order_price { get; set; }

        public string UserEmail { get; set; }

        public int Promotion_discount_id { get; set; }

        public int Customer_id { get; set; }
        public string Customer_Address { get; set; }



    }
}
