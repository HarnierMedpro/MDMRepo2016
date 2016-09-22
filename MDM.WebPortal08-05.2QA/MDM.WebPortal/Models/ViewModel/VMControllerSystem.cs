﻿using MDM.WebPortal.Data_Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMControllerSystem
    {
        public int ControllerID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "This field does not allow empty strings.")]
        [RegularExpression(@"[a-zA-Z0-9_]+$", ErrorMessage = "No Special character and/or white space allowed.")]
        [Display(Name = "CONTROLLER NAME")]
        [StringLength(50)]
        public string Cont_Name { get; set; }

        [Display(Name = "AREA")]
        public int AreaID { get; set; }

       
    }
}