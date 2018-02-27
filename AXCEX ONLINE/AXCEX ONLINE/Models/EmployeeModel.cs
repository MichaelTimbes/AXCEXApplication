using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AXCEX_ONLINE.Models
{
    // Employee Model Inherits From Application User
    public class EmployeeModel : ApplicationUser
    {
        
        public int EMPID { get; set; }
        
        // Different From the User Name that is Found in Application User
        public string EMP_FNAME { get; set; }
        public string EMP_LNAME { get; set; }
        
    }
}
