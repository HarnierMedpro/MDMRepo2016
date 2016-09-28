using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
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