using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace FYP.Models
{
    public class Order
    {
        //auto generated
        public int Order_id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Order_Date { get; set; }

        [Required]
        //[DataType(DataType.Time)]
        public string Order_Time { get; set; }
        
        [Required]
        public string Order_Status { get; set; }

        [Required]
        public double Order_price { get; set; }
        
        public string UserEmail { get; set; }

        public int Promotion_discount_id { get; set; }

        public int Payment_id { get; set; }


    }

}
