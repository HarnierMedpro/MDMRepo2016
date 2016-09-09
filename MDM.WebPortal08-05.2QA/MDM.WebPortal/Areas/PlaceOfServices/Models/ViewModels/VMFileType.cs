using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMFileType
    {
        public int FileTypeID { get; set; }

        [Display(Name = "FILE TYPE")]
        [Required]
        [StringLength(50)]
        public string FileType_Name { get; set; }
    }
}