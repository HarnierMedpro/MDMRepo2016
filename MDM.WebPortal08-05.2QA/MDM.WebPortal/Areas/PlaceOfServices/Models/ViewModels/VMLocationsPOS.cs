using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMLocationsPOS
    {
        public string PosName { get; set; }

        public int Facitity_DBs_IDPK { get; set; }
       

        public int FvPList_FvPID { get; set; }

        public int FACInfoData_FACInfoDataID { get; set; }

        public Nullable<int> PHYGroups_PHYGrpID { get; set; }

        [Display(Name = "TAXID")]
        [StringLength(80)]
        public string TaxID { get; set; }

        public string DBA_Name { get; set; }

        [Display(Name = "PAYMENT ADDRESS1")]
        public string Payment_Addr1 { get; set; }

        [Display(Name = "PAYMENT ADDRESS2")]
        public string Payment_Addr2 { get; set; }

        [Display(Name = "PAYMENT CITY")]
        public string Payment_City { get; set; }

        [Display(Name = "PAYMENT STATE")]
        public string Payment_state { get; set; }

        [Display(Name = "PAYMENT ZIP")]
        public string Payment_Zip { get; set; }

        [Display(Name = "PHYSCAL ADDRESS1")]
        public string Physical_Addr1 { get; set; }

        [Display(Name = "PHYSCAL ADDRESS2")]
        public string Physical_Addr2 { get; set; }

        [Display(Name = "PHYSCAL CITY")]
        public string Physical_City { get; set; }

        [Display(Name = "PHYSCAL STATE")]
        public string Physical_state { get; set; }

        [Display(Name = "PHYSCAL ZIP")]
        public string Physical_Zip { get; set; }

        [Display(Name = "ACCOUNT MANAGER")]
        public string POSFAC_Manager { get; set; }

        [Display(Name = "NOTES")]
        public string Notes { get; set; }
    }
}