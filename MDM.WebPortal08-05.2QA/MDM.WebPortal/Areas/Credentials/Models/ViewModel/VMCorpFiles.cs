using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMCorpFiles
    {
        public int FileID { get; set; }//PK
        public int corpID { get; set; } //FK from dbo.CorporateMasterList table
        public int MasterPOSID { get; set; }

        [Display(Name = "FILE NAME")]
        public string FileName { get; set; }

        [Display(Name = "EXTENSION")]
        public string FileExtension { get; set; }

        [Required]
        [Display(Name = "DESCRIPTION")]
        public string Description { get; set; }
        
        [Display(Name = "FILE TYPE")]
        public int FileTypeID { get; set; }//FK for the type of File
    }
}