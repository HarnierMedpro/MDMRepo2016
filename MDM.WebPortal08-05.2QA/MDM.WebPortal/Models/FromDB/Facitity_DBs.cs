//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MDM.WebPortal.Models.FromDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Facitity_DBs
    {
        public int IDPK { get; set; }
        public string DB { get; set; }
        public string DatabaseName { get; set; }
        public Nullable<long> Facility_ID { get; set; }
        public string Fac_NAME { get; set; }
        public bool Active { get; set; }
    
        public virtual LocationsPOS LocationsPOS { get; set; }
    }
}
