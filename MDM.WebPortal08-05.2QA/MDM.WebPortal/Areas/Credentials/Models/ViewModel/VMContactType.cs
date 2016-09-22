using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMContactType
    {
        public int ContactTypeID { get; set; }

        [Required]
        [Display(Name = "CONTACT TYPE")]
        [StringLength(50)]
        public string ContactType_Name { get; set; }

        [Required]
        [Display(Name = "LEVEL")]
        [StringLength(50)]
        public string ContactLevel { get; set; }
    }
}