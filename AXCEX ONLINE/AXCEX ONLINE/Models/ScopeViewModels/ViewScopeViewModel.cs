using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.ScopeViewModels
{
    public class ViewScopeViewModel
    {
        // Project Name
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        // Assigned Manager
        [Display(Name = "Scope Manager")]
        public string ScopeManager{ get; set; }
        
        // Declaration of Expectations
        [Display(Name = "SCOPE EXPECTATIONS")]
        [DataType(DataType.MultilineText)]
        public string UpdatedScopeExpectations { get; set; }

        // Declaration of Limitations
        [Display(Name = "SCOPE IMITATIONS")]
        [DataType(DataType.MultilineText)]
        public string UpdatedScopeLimitations { get; set; }

        // Scope Summary
        [Display(Name = "SCOPE SUMMARY")]
        [DataType(DataType.MultilineText)]
        public string UpdatedScopeSummary { get; set; }

        // Declaration of Goals
        [Display(Name = "SCOPE GOALS")]
        [DataType(DataType.MultilineText)]
        public string UpdatedScopeGoals { get; set; }

        // Phase of the Project
        [Display(Name = "CURRENT SCOPE PHASE NAME")]
        public string UpdatedScopePhase { get; set; }

        // Current Phase Number of the Project
        [Display(Name = "CURRENT SCOPE PHASE (NUMERICAL)")]
        public int UpdatedScopePhaseNum { get; set; }

        // Numerical Maximum Phase of the Project
        [Display(Name = "SCOPE MAX PHASE NUMBER")]
        public int UpdatedScopePhaseNumberMax { get; set; }

        // Start and End Date
        [Display(Name = "SCOPE START DATE")]
        [DataType(DataType.Date)]
        public DateTime ScopeStartDate { get; set; }

        [Display(Name = "SCOPE END DATE")]
        [DataType(DataType.Date)]
        public DateTime ScopeEndDate { get; set; }


        // AUTO AND HIDDEN
        public int ScopeCurrentVersion { get; set; } // TO BE USED IN AUTO INCREMENT
        public string ScopeAuthor { get; set; } // HIDDEN
        public int ProjectId { get; set; } // HIDDEN
        public int ScopeParent { get; set; } // Previous Scope ID

    }
}
