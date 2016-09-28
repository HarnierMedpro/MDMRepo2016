using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMMPService
    {
        public int MPServID { get; set; }

        [Required]
        [Display(Name = "SERVICE")]
        [StringLength(80)]
        [RegularExpression(@"[a-zA-Z0-9_]+$", ErrorMessage = "No Special character and/or white space allowed.")]
        public string ServName { get; set; }
    }
}