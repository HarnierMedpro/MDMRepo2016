using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMFormsDict
    {
        public int FormsID { get; set; }

        [Required]
        [Display(Name = "NAME")]
        [StringLength(20)]
        public string FormName { get; set; }
    }
}