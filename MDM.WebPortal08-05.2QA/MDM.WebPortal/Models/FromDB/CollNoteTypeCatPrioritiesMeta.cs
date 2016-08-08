using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class CollNoteTypeCatPrioritiesMeta
    {
        [DisplayName("ACTION DESCRIPTION")]
        [StringLength(80)]
        [Required]
        public string CollNoteType { get; set; }

        [DisplayName("ACTION CODE")]
        [StringLength(50)]
        [Required]
        public string Code { get; set; }

        [DisplayName("ACTION CATEGORY")]
        [StringLength(50)]
        [Required]
        public string CollNoteCat { get; set; }

        [Required]
        [Display(Name = "PRIORITY")]
        [StringLength(50)]
        public string Priority { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }
    }

    [MetadataTypeAttribute(typeof(CollNoteTypeCatPrioritiesMeta))]
    public partial class CollNoteTypeCatPriority
    {
    }
}