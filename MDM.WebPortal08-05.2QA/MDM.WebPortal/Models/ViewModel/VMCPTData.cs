using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMCPTData
    {
        public int id { get; set; }

        [Required]
        [Display(Name = "CPT")]
        [StringLength(5)]
        [RegularExpression(@"[0-9]+$", ErrorMessage = "Only numbers are allowed.")]
        public string CPT { get; set; }

        [Required]
        [Display(Name = "DESCRIPTION")]       
        public string CPT_Description { get; set; }

        [Display(Name = "SHORT DESCRIPTION")]
        [StringLength(255)]
        public string ShortD { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }
    }
}