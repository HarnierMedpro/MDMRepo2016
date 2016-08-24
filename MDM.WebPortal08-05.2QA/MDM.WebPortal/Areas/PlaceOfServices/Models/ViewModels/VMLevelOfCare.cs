using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMLevelOfCare
    {
        public int LevOfCareID { get; set; }

        [Required]
        [Display(Name = "LEVEL OF CARE")]
        [StringLength(80)]
        //[RegularExpression(@"[a-zA-Z0-9_]+$", ErrorMessage = "No Special character and/or white space allowed.")]
        public string LevOfCareName { get; set; }
    }
}