using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.EmployeeViewModels
{
    public class EmployeeViewAssignedProjectsVM
    {
        // Show the Project Name 
        [Display(Name = "Project")]
        public string Project_Name { get; set; }

        // Who Assigned the Project
        [Display(Name = "Assigned By")]
        public string Assigned_By { get; set; }
        
        // The Start Date
        [Display(Name = "Start")]
        public DateTime Start_Date { get; set; }
        
        // The Due Date
        [Display(Name = "Due")]
        public DateTime Due_Date { get; set; }

        // For REF
        public int Project_ID { get; set; }
        // FUTURE: Current Scope
        // FUTURE: Assigned Tasks 
    }
}
