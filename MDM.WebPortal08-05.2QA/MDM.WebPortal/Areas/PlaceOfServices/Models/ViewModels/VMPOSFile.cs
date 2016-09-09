using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMPOSFile
    {
        
        public int FileID { get; set; }

        public int Facitity_DBs_IDPK { get; set; }

        //FK for the type of File
        [Display(Name = "FILE TYPE")]
        public int FileTypeID { get; set; }

        [Required]
        [Display(Name = "DESCRIPTION")]
        public string Description { get; set; }
    }
}