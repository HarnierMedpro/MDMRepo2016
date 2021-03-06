﻿using System.ComponentModel.DataAnnotations;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMLocationPOS_AddressInfo
    {
        public int MasterPOSID { get; set; } //PK from dbo.MasterPOS

        public int POSAddrID { get; set; }

        [Required]
        [Display(Name = "DBA NAME")]
        public string DBA_Name { get; set; }

        [Required]
        [Display(Name = "ADDRESS 1")]
        public string Payment_Addr1 { get; set; }

        [Display(Name = "ADDRESS 2")]
        public string Payment_Addr2 { get; set; }

        [Required]
        [Display(Name = "CITY")]
        public string Payment_City { get; set; }

        [Required]
        [Display(Name = "STATE")]
        public string Payment_state { get; set; }

        [Required]
        [Display(Name = "ZIP")]
        [StringLength(5, MinimumLength = 5)]
        [RegularExpression(@"[0-9]+$", ErrorMessage = "Only numbers allowed.")]
        public string Payment_Zip { get; set; }

        [Required]
        [Display(Name = "ADDRESS 1")]
        public string Physical_Addr1 { get; set; }

        [Display(Name = "ADDRESS 2")]
        public string Physical_Addr2 { get; set; }

        [Required]
        [Display(Name = "CITY")]
        public string Physical_City { get; set; }

        [Required]
        [Display(Name = "STATE")]
        public string Physical_state { get; set; }

        [Required]
        [Display(Name = "ZIP")]
        [StringLength(5, MinimumLength = 5)]
        [RegularExpression(@"[0-9]+$", ErrorMessage = "Only numbers allowed.")]
        public string Physical_Zip { get; set; }
       
        [Display(Name = "NOTES")]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "TIME ZONE")]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression(@"[a-zA-Z]+$", ErrorMessage = "No Special character, white and/or numbers space allowed.")]
        public string Time_Zone { get; set; }
    }
}