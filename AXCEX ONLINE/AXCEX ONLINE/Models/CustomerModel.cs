using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models
{
    public class CustomerModel : ApplicationUser
    {
        public int ID { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public int CUSTOMER_ACCOUNT { get; set; }

    }
}
