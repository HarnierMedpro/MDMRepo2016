
using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMFileType
    {
        public int FileTypeID { get; set; }

        [Display(Name = "FILE TYPE")]
        [Required]
        [StringLength(50)]
        public string FileType_Name { get; set; }

        [Required]
        [Display(Name = "FILE LEVEL")]
        public string FileLevel { get; set; }
    }
}