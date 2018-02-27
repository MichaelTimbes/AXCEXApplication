using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models
{
    // Inherits From Application User for Authentication
    public class AdminModel : ApplicationUser
    {
        public int ADMIN_ID { get; set; }
        public string ADMIN_NAME { get; set; }
        

    }
}
