using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMMenu
    {
        public int MenuID { get; set; }

        [Display(Name = "PARENT MENU")]
        public Nullable<int> ParentId { get; set; }

        [Display(Name = "ACTION")]
        //[UIHint("ActionsInSystem")]
        public Nullable<int> ActionID { get; set; }

        [Required]
        [Display(Name = "TITLE")]
        [StringLength(50)]
        public string Title { get; set; }
    }
}