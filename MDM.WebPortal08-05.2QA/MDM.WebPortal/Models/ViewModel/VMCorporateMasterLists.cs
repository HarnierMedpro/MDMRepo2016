using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMCorporateMasterLists
    {
        public int corpID { get; set; }

        [Required]
        [Display(Name = "CORPORATE NAME")]
        [StringLength(100)]
        public string CorporateName { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public Nullable<bool> active { get; set; }
    }
}