using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;


namespace FYP.Models
{
    public class VisitorRegistration
    {
        // Using email as PK
        [Required(ErrorMessage = "Please enter your Email")]
        [Remote(action: "VerifyUserEmail", controller: "Account")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please enter Full Name")]
        [StringLength(45, MinimumLength = 6, ErrorMessage = "Full Name must be 6 characters or more")]
        public string User_fullname { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [StringLength(45, MinimumLength = 5, ErrorMessage = "Password must be 5 characters or more")]
        public string User_Password { get; set; }

        [Compare("User_Password", ErrorMessage = "Passwords do not match")]
        public string User_Password2 { get; set; }

        public string User_type { get; set; }

        public string Status { get; set; }

        public DateTime LastLogin { get; set; }

        public string StopLogin { get; set; }

        public string Inactive_status { get; set; }
    }

}
