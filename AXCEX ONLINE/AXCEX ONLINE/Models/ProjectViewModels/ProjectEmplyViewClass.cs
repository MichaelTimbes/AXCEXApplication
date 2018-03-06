using System;
using System.ComponentModel.DataAnnotations;

namespace AXCEX_ONLINE.Models.ProjectViewModels
{
    public class ProjectDetailsViewClass
    {
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
    }
}
