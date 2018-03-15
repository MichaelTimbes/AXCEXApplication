using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models.CustomerViewModels
{
    public class CustomerHomeVM
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int NumProjects { get; set; }
        
    }
}
