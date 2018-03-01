using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models
{
    public class ProjectModel
    {
        // Project Model Based on the Db Design

        // PK
        public int ProjID { get; set; }
        
        // ApplicationUserID FK -> Application User
        // ApplicationUser Navigation
        public int ApplicationUserID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        // Employee's Assigned to the Project
        public ICollection<EmployeeModel> AssignedEmployees { get; set; }

        // Project's Budget
        [Column(TypeName = "money")]
        public double ProjBudget { get; set; }

        // Project's Current Cost
        [Column(TypeName = "money")]
        public double ProjCurentCost { get; set; }


    }
}
