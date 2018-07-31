using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AXCEX_ONLINE.Models;

namespace AXCEXONLINE.Models.ProjectViewModels
{
    public class ProjectDetailsViewClass
    {
        public int ProjectId { get; set; }
        
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Customer Name")]
        public string Custid { get; set; }

        [Display(Name = "Project Budget")]
        [DataType(DataType.Currency)]
        public decimal ProjBudget { get; set; }


        [Display(Name = "Current Project Cost")]
        [DataType(DataType.Currency)]
        public decimal ProjCost { get; set; }

        public bool ActiveProj { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime ProjStart { get; set; }


        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime ProjEnd { get; set; }

        [Display(Name = "Employees Assigned")]
        public List<EmployeeModel> Employees = new List<EmployeeModel>();

        [Display(Name = "Current Scope")]
        public ScopeModel CurrentScope { get; set; }



    }
}
