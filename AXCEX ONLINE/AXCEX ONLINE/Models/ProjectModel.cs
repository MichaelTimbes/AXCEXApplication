﻿using System;
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
        [Column(name:"ProjectID")]
        public int ID { get; set; }

        [Column(name: "ProjectName")]
        public string ProjectName { get; set; }

        // ApplicationUserID FK -> Application User
        // ApplicationUser Navigation
        [Column(name: "CustomerName")]
        public string Customer { get; set; }

        // Employee's Assigned to the Project
        public ICollection<EmployeeModel> AssignedEmployees { get; set; }

        // Project's Budget
        [Column(TypeName = "money")]
        public decimal ProjBudget { get; set; }

        // Project's Current Cost
        [Column(TypeName = "money")]
        public decimal ProjCurentCost { get; set; }


    }
}