using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMLocationPOS_AddressInfo
    {
        public int Facitity_DBs_IDPK { get; set; } //PK from dbo.LocationsPOS

        [Display(Name = "ADDRESS 1")]
        public string Payment_Addr1 { get; set; }

        [Display(Name = "ADDRESS 2")]
        public string Payment_Addr2 { get; set; }

        [Display(Name = "CITY")]
        public string Payment_City { get; set; }

        [Display(Name = "STATE")]
        public string Payment_state { get; set; }

        [Display(Name = "ZIP")]
        public string Payment_Zip { get; set; }

        [Display(Name = "ADDRESS 1")]
        public string Physical_Addr1 { get; set; }

        [Display(Name = "ADDRESS 2")]
        public string Physical_Addr2 { get; set; }

        [Display(Name = "CITY")]
        public string Physical_City { get; set; }

        [Display(Name = "STATE")]
        public string Physical_state { get; set; }

        [Display(Name = "ZIP")]
        public string Physical_Zip { get; set; }

        [Display(Name = "NOTES")]
        public string Notes { get; set; }

        [Display(Name = "TIME ZONE")]
        public string Time_Zone { get; set; }
    }
}