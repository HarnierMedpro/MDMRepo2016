using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ActionCode.Models.ViewModels
{
    public class VMACType
    {
        public int ACTypeID { get; set; }

        [Required]
        [Display(Name = "ACTYPE NAME")]
        [StringLength(80)]
        public string ACTypeName { get; set; }
    }
}