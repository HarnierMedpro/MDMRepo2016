using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
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