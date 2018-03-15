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
        [Display(Name = "Customer Email")]
        [DataType(DataType.EmailAddress)]
        public string CustEmail{ get; set; }

        [Required]
        [Display(Name = "Project Budget")]
        [DataType(DataType.Currency)]
        public decimal ProjBudget { get; set; }


        [Required]
        [Display(Name = "Current Project Cost")]
        [DataType(DataType.Currency)]
        public decimal ProjCost { get; set; }

        public bool ActiveProj = true;

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime ProjStart { get; set; }


        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime ProjEnd { get; set; }

        
    }
}
