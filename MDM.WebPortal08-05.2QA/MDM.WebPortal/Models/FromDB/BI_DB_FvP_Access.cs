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
    
    public partial class BI_DB_FvP_Access
    {
        public int BIDbFvPID { get; set; }
        public int ManagerID { get; set; }
        public int DB_ID { get; set; }
        public int FvPID { get; set; }
        public bool Active { get; set; }
    
        public virtual FvPList FvPList { get; set; }
        public virtual Manager_Master Manager_Master { get; set; }
        public virtual DBList DBList { get; set; }
    }
}
