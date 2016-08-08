using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class CorporateNameListsMeta
    {
        [Required]
        [StringLength(255)]
        [Display(Name = "DB")]
        public string DBno { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "CORPORATE NAME")]
        public string CorporateName { get; set; }
    }
    [MetadataTypeAttribute(typeof(CorporateNameListsMeta))]
    public partial class CorporateNameList
    {
    }
}