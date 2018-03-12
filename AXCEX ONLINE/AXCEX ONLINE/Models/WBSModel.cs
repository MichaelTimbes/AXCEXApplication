using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models
{
    public class WBSModel
    {
        // PK
        [Column(name: "WBS_ID")]
        public int ID { get; set; }

        // Technically a FK
        [Column(name: "Project_ID")]
        public int ProjectId { get; set; }
        
        // Who assigned the WBS
        [Column(name: "WBS_ASSIGNED_BY")]
        public string AssignedBy { get; set; }
        
        // Overview or Summary
        [Column(name: "WBS_SUMMARY")]
        public string WBSSummary { get; set; }

        // Number of Hours
        [Column(name: "MAX_HOURS")]
        public double WBSHours { get; set; }

        // Start and End Date
        [Column(name: "WBS_START_DATE")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        
        [Column(name: "WBS_END_DATE")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // Cost
        [Column(name: "WBS_TOTAL_COST", TypeName = "money")]
        public decimal WBSCost { get; set; }

    }
}
