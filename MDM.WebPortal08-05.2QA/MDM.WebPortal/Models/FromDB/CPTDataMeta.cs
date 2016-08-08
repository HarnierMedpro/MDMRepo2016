using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class CPTDataMeta
    {
        [Required]
        [Display(Name = "CPT")]
        [StringLength(10)]
        public string CPT { get; set; }

        [Required]
        [Display(Name = "CPT DESCRIPTION")]
        public string CPT_Description { get; set; }

        [Display(Name = "SHORT DESCRIPTION")]
        [StringLength(255)]
        public string ShortD { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public Nullable<bool> Active { get; set; }
    }
    [MetadataType(typeof(CPTDataMeta))]
    public partial class CPTData
    {
    }
}