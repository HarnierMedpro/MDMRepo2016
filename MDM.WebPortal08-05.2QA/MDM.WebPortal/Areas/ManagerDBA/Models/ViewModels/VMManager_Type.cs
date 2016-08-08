using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels
{
    public class VMManager_Type
    {
        public int ManagerTypeID { get; set; }

        [Required]
        [Display(Name = "CLASSIFICATION")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }
    }
}