﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMDBList
    {
        public int DB_ID { get; set; }

        [Required]
        [StringLength(3, ErrorMessage = "This field must have a maximum length of 3.")]
        public string DB { get; set; }


        [Display(Name = "NAME")]
        [StringLength(75)]
        public string databaseName { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public Nullable<bool> active { get; set; }

        public VMDBList() { }
    }
}