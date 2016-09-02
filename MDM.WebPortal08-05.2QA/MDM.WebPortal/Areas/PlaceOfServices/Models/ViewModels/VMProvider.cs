using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMProvider
    {
        public int ProvID { get; set; }

        [Required]
        [Display(Name = "PROVIDER NAME")]
        [StringLength(80)]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Provider Name.")]
        public string ProviderName { get; set; }

        [Required]
        [Display(Name = "NPI NUMBER")]
        [RegularExpression(@"[a-zA-Z0-9]+$", ErrorMessage = "No Special character and/or white space allowed.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "The NPI Number needs to have a length of 10.")]
        public string NPI_Num { get; set; }
    }
}