using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MDM.WebPortal.Models.FromDB;

namespace MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels
{
    public class VMManager_BI
    {
        public int ManagerID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "ALIAS NAME")]
        public string AliasName { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }

        [Display(Name = "MANAGER")]
        public string Classification { get; set; }

        public Manager_Type Manager_Type { get; set; }
    }
}