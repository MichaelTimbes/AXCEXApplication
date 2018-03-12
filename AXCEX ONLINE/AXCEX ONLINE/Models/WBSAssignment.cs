using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models
{
    public class WBSAssignment
    {
        // PK
        [Column(name: "WBS_ASSIGNMENT_ID")]
        public int ID { get; set; }

        // FK Employee ID
        [Column(name: "EMPLOYEE_ID")]
        public int EmpKey { get; set; }

        // Project ID
        [Column(name: "WBS_ID")]
        public int WBSKey { get; set; }

        // Who Authorized the Assignment
        [Column(name: "ASSIGNING_MANAGER")]
        public string AuthorizedBy { get; set; }

        // Is the WBS Complete?
        [Column(name: "WBS_COMPLETION_STAT")]
        public bool IsComplete { get; set; }

        [Column(name: "WBS_AWAITING_COMPLETION_STAT")]
        public bool IsCompleteAwaiting { get; set; }

        // Verified Completed
        [Column(name: "WBS_COMPLETION_VERIFIED")]
        public bool IsCompleteVerified { get; set; }



    }
}
