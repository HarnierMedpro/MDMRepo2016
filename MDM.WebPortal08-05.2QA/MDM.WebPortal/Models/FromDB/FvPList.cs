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
    
    public partial class FvPList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FvPList()
        {
            this.BI_DB_FvP_Access = new HashSet<BI_DB_FvP_Access>();
        }
    
        public int FvPID { get; set; }
        public string FvPName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BI_DB_FvP_Access> BI_DB_FvP_Access { get; set; }
    }
}