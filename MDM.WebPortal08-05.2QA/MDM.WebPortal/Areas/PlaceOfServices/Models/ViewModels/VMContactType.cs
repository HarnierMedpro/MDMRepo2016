using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMContactType
    {
        public int ContactTypeID { get; set; }

        [Required]
        [Display(Name = "TYPE")]
        [StringLength(50)]
        public string ContactType_Name { get; set; }
    }
}