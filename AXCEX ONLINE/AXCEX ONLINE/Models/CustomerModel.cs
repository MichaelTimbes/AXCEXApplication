using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Models
{
    /* Used to Keep Track of Customers.
     * Will Auto Populate when a Customer Registers an Account.
     */
    public class CustomerModel
    {
        // Unique ID
        [Column(name: "CUSTOMER_ID")]
        public int ID { get; set; }

        // First and Last Name
        [Column(name: "CUSTOMER_NAME")]
        public string CUSTOMER_NAME { get; set; }

        // This is to be set by the admin
        [Column(name: "CUSTOMER_ACCOUNT_NUM")]
        public string CUSTOMER_ACCOUNT { get; set; }

    }
}
