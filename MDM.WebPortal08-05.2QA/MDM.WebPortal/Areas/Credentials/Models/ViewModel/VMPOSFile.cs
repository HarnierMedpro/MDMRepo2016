using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMPOSFile
    {
        public int FileID { get; set; }//PK
        public int MasterPOSID { get; set; } //FK from dbo.MasterPOS table

        [Display(Name = "FILE NAME")]
        public string FileName { get; set; }

        [Display(Name = "EXTENSION")]
        public string FileExtension { get; set; }

        [Required]
        [Display(Name = "DESCRIPTION")]
        public string Description { get; set; }

        //FK for the type of File
        [Display(Name = "FILE TYPE")]
        public int FileTypeID { get; set; }
    }
}