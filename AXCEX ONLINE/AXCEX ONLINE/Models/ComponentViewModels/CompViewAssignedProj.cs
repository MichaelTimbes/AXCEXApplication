using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.ComponentViewModels
{
    public class CompViewAssignedProj
    {
        // Show the Project Name 
        [Display(Name = "Project")]
        public string Project_Name { get; set; }

        // Who Assigned the Project
        [Display(Name = "Assigned By")]
        public string Assigned_By { get; set; }

        [Display(Name = "Project Active")]
        public bool ActiveProj { get; set; }


        // FUTURE: Current Scope
        // FUTURE: Assigned Tasks 
    }
}
