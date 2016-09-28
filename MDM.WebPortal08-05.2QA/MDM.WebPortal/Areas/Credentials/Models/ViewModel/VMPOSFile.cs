using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMPOSFile
    {
        
        public int FileID { get; set; }

        public int MasterPOSID { get; set; }

        //FK for the type of File
        [Display(Name = "FILE TYPE")]
        public int FileTypeID { get; set; }

        [Required]
        [Display(Name = "DESCRIPTION")]
        public string Description { get; set; }

        [Display(Name = "EXTENSION")]
        public string FileExtension { get; set; }
    }
}