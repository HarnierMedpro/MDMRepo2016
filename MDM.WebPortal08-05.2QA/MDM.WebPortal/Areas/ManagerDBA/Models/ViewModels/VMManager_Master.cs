using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels
{
    public class VMManager_Master
    {
        public int ManagerID { get; set; }

        public int ManagerTypeID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "ALIAS NAME")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Alias Name.")]
        public string AliasName { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }
    }
}