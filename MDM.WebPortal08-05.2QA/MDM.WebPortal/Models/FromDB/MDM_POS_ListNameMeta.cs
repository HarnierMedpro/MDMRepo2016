using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class MDM_POS_ListNameMeta
    {
        [Required]
        [Display(Name ="NAME")]
        [StringLength(255)]
        public string PosName { get; set; }
    }
    [MetadataType(typeof(MDM_POS_ListNameMeta))]
    public partial class MDM_POS_ListName
    {

    }
}