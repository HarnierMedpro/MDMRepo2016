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
    
    public partial class PHYGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PHYGroup()
        {
            this.ProvidersInGrps = new HashSet<ProvidersInGrp>();
            this.LocationsPOS = new HashSet<LocationsPOS>();
        }
    
        public int PHYGrpID { get; set; }
        public string PHYGroupName { get; set; }
        public string PHYGrpNPI_Num { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProvidersInGrp> ProvidersInGrps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LocationsPOS> LocationsPOS { get; set; }
    }
}
