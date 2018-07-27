using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.WBSViewModels

{
    public class WBSCreateVM
    {
        // Technically a FK
        [Display(Name = "Project_ID")]
        public int ProjectId { get; set; }

        // Who assigned the WBS
        [Required]
        [Display(Name= "WBS_ASSIGNED_BY")]
        public string AssignedBy { get; set; }

        // Who is assigned the WBS
        //[Display(Name = "Employee List")]
        //public List<EmployeeModel> EmployeeList = new List<EmployeeModel>();
        //public IEnumerable<ApplicationUser> EmployeeList { get; set; }
        
        // Assigned to Employee
        //[Display(Name = "Employee")]
       // public string EMPID { get; set; }

        // Overview or Summary
        [Required]
        [Display(Name= "WBS_SUMMARY")]
        [DataType(DataType.MultilineText)]
        public string WBSSummary { get; set; }

        // Number of Hours
        [Required]
        [Display(Name= "MAX_HOURS")]
        public double WBSHours { get; set; }

        // Start and End Date
        [Required]
        [Display(Name= "WBS_START_DATE")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name= "WBS_END_DATE")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Cost
        [Display(Name= "WBS_TOTAL_COST")]
        [DataType(DataType.Currency)]
        public decimal WBSCost { get; set; }

        
    }
}
