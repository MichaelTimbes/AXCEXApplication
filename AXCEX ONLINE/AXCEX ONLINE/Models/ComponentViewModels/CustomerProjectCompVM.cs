using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.ComponentViewModels
{
    public class CustomerProjectCompVM
    {
        // Show the Project Name 
        [Display(Name = "Project")]
        public string Project_Name { get; set; }

        // Project Progress
        [Display(Name = "Phase")]
        public int ProgressPhaseCurrent{ get; set; }

        // Project Progress
        [Display(Name = "Max Phase")]
        public int ProgressPhaseMax { get; set; }

        // Project Percentage Progress
        [Display(Name = "Percent Complete")]
        public decimal ProgressPercent { get; set; }
        // Display if Active- Optional
        [Display(Name = "Project Active")]
        public bool ActiveProj { get; set; }

        // Hidden Information For Use in Razor URL 
        public int ScopeId { get; set; }
        public int ProjectId { get; set; }

    }
}
