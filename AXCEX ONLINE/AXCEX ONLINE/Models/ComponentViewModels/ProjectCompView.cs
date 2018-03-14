using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.ComponentViewModels
{
    public class ProjectCompView
    {
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        public int ProjectId { get; set; }

        [Display(Name = "Project Active")]
        public bool ActiveProj { get; set; }
    }
}
