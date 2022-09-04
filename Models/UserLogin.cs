using System.ComponentModel.DataAnnotations;

namespace FYP.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter your Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please enter your Password")]
        [DataType(DataType.Password)]
        public string User_Password { get; set; }

        public bool RememberMe { get; set; }

        public string StopLogin { get; set; }

    }
}
