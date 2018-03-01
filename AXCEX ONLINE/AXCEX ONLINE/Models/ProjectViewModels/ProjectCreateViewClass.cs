using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.ProjectViewModels
{
    public class ProjectCreateViewClass
    {
        [Required]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string Custid { get; set; }

        [Required]
        [Display(Name = "Project Budget")]
        [DataType(DataType.Currency)]
        public decimal ProjBudget { get; set; }


        [Required]
        [Display(Name = "Current Project Cost")]
        [DataType(DataType.Currency)]
        public decimal ProjCost { get; set; }

    }
}
