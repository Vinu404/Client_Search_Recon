using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClientserachUtiliy.Models
{
    public class Login
    {
       
        public string UserID { get; set; }
        [Required(ErrorMessage = "username is Required")]
        [Display(Name = "USERNAME")]
        public string username { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [Display(Name = "PASSWORD")]
        public string Password { get; set; }
    }
}