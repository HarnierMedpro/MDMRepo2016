﻿using System;
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
        [Display(Name = "CODE")]
        public string Code { get; set; }
    }
}