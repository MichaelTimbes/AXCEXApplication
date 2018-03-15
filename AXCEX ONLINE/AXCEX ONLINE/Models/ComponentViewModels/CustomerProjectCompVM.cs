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

        // Who Assigned the Project
        [Display(Name = "Progress")]
        public double ProgressPercentage{ get; set; }
        
        // Display if Active- Optional
        [Display(Name = "Project Active")]
        public bool ActiveProj { get; set; }

        // Hidden Information For Use in Razor URL 
        public int ScopeId { get; set; }
        public int ProjectId { get; set; }

    }
}
