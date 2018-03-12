using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models
{
    public class ScopeModel
    {
    
        // PK
        [Column(name: "SCOPE_ID")]
        public int ID { get; set; }

        // FK ref
        [Column(name: "Project_ID")]
        public int ProjectId { get; set; }

        // Version Number
        [Column(name: "SCOPE_VERSION")]
        public int ScopeVersion { get; set; }

        // Declaration of Goals
        [Column(name: "SCOPE_VERSION_AUTHOR")]
        public string ScopeAuthor { get; set; }

        // Declaration of Goals
        [Column(name: "SCOPE_MANAGER")]
        public string ScopeManager { get; set; }

        // Declaration of Goals
        [Column(name: "SCOPE_EXPECTATIONS")]
        public string ScopeExpectations { get; set; }

        // Declaration of Goals
        [Column(name: "SCOPE_LIMITATIONS")]
        public string ScopeLimitations { get; set; }

        // Declaration of Goals
        [Column(name: "SCOPE_SUMMARY")]
        public string ScopeSummary { get; set; }

        // Declaration of Goals
        [Column(name: "SCOPE_GOALS")]
        public string ScopeGoals { get; set; }

        // Phase of the Project
        [Column(name: "SCOPE_PHASE")]
        public string ScopePhase { get; set; }

        // Start and End Date
        [Column(name: "SCOPE_START_DATE")]
        [DataType(DataType.Date)]
        public DateTime ScopeStartDate { get; set; }

        [DataType(DataType.Date)]
        [Column(name: "SCOPE_END_DATE")]
        public DateTime ScopeEndDate { get; set; }
    }
}
