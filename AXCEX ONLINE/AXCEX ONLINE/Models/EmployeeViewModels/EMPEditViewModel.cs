using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.EmployeeViewModels
{
    public class EMPEditViewModel
    {
        [Display(Name = "First Name")]
        public string Employee_fname { get; set; }

        [Display(Name = "Last Name")]
        public string Employee_lname { get; set; }

        [Display(Name = "User Name")]
        public string Employee_userName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

    }
}
