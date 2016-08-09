using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ActionCode.Models.ViewModels
{
    public class VMCodeMasterList
    {
        public int CodeID { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }
    }
}