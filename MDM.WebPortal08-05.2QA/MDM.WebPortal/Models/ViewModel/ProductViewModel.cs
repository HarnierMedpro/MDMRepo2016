using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class ProductViewModel
    {
        public int id { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        //public decimal? UnitPrice { get; set; }
        public string UnitsOnOrder { get; set; }
        public string UnitsInStock { get; set; }



        //public int id { get; set; }
        //public string ProductName { get; set; }
        //public Nullable<decimal> UnitPrice { get; set; }
        //public string UnitsOnOrder { get; set; }
        //public string UnitsInStock { get; set; }
    }
}


//ProductName
//UnitPrice
//UnitsOnOrder
//UnitsInStock