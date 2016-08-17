using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMAreaSystems
    {
        public int AreaID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "This field does not allow empty strings.")]
        [RegularExpression(@"[a-zA-Z0-9_]+$", ErrorMessage = "No Special character and/or white space allowed.")]
        [Display(Name = "AREA NAME")]
        [StringLength(50)]
        public string AreaName { get; set; }
    }
}