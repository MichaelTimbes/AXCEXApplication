using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models
{
    public class ProjectAssignment
    {
        
        public int ID { get; set; }
        // Employee ID
        public int EmpKey { get; set; }
        // Project ID
        public int ProjKey { get; set; }
        // Who Authorize the Assignment
        public string authorized_assignment { get; set; }
    }
}
