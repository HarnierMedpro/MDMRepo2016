using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ActionCode.Models.ViewModels
{
    public class VMCategoria
    {
        public int CatogoryID { get; set; }

        [Required]
        [Display(Name = "CATEGORY NAME")]
        [StringLength(50)]
        public string CategoryName { get; set; }
    }
}