using System;
using System.ComponentModel.DataAnnotations;

namespace AXCEX_ONLINE.Models.ProjectViewModels
{
    public class ProjectEmplyCompViewClass
    {
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        public int ProjectId { get; set; }

        [Display(Name = "Project Active")]
        public bool ActiveProj { get; set; }
    }
}
