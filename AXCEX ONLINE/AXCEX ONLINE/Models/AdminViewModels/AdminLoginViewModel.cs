using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.AdminViewModels
{
    public class AdminLoginViewModel
    {
        

        [Required]
        [Display(Name = "Admin UserName")]
        public string Admin_Uname { get; set; }

        [Required]
        [Display(Name ="Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe = false;

    }
}
