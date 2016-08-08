using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class CorporateMasterListMeta
    {
        [Required]
        [Display(Name="NAME")]
        [StringLength(100, ErrorMessage = "This field must be a string with a maximum length of 100.")]
        public string CorporateName { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public Nullable<bool> active { get; set; }
    }

    [MetadataType(typeof(CorporateMasterListMeta))]
    public partial class CorporateMasterList
    {
    }
}