using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/* View Model For:
 * /Project/AssignEmployee/[projectID]
*/
namespace AXCEX_ONLINE.Models.ProjectViewModels
{
    public class ProjectAssignEmpViewModel
    {
        public int Project_ID { get; set; }
        public int Employee_ID { get; set; }
        public string Employee_Name { get; set; }
        public bool IsAssigned { get; set; }
    }
}
