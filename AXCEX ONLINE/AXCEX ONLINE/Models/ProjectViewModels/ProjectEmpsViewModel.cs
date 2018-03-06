using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/* View Model For:
 * /Project/AssignedEmployees/[projectID]
*/
namespace AXCEX_ONLINE.Models.ProjectViewModels
{
    public class ProjectEmpsViewModel
    {
        // Used to delete an assignment
        public int Assignment_ID { get; set; }
        // Reference to the project
        public int Project_ID { get; set; }
       
        public int Employee_ID { get; set; }
        public string Employee_Name { get; set; }
    }
}
