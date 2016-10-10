using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ActionCode.Models.ViewModels
{
    public class VMActionCode
    {
        public int ActionCodeID { get; set; }//PK

        [Required]
        [StringLength(80)]
        [Display(Name = "COLLECTION NOTE TYPE")]
        public string CollNoteType { get; set; }

        [Display(Name = "CODE")]
        [UIHint("ACCodeEditor")]
        public int CodeID { get; set; }

        [Display(Name = "CATEGORY")]
        public int CategoryID { get; set; }

        [Display(Name = "PRIORITY")]
        public int PriorityID { get; set; }

        [Display(Name = "ACType")]
        public int ACTypeID { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "PARSING")]
        public string ParsingYN { get; set; }
    }
}