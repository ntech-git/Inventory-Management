using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory_System.Models
{
    public class Products
    {
        public int pid { get; set; }
        public string name { get; set; }

        public string category { get; set; }

        public string description { get; set; }

        public  int price { get; set; }
        public HttpPostedFileBase image { get; set; }
        public string file { get; set; }
    }
}