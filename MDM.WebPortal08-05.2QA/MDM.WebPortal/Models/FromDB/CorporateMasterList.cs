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
    
    public partial class CorporateMasterList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CorporateMasterList()
        {
            this.Corp_DBs = new HashSet<Corp_DBs>();
            this.Corp_Owner = new HashSet<Corp_Owner>();
        }
    
        public int corpID { get; set; }
        public string CorporateName { get; set; }
        public Nullable<bool> active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Corp_DBs> Corp_DBs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Corp_Owner> Corp_Owner { get; set; }
    }
}
