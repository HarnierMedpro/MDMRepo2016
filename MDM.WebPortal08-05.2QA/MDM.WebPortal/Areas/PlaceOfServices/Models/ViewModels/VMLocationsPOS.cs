using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMLocationsPOS
    {
        public int Facitity_DBs_IDPK { get; set; } //PK from dbo.LocationsPOS

        [Display(Name = "POS CLASSIFICATION")]
        public int FvPList_FvPID { get; set; } //FK from dbo.FvPLists table. Could be FAC or PHY by default its FAC

        public int FACInfoData_FACInfoDataID { get; set; } //FK from dbo.FACInfoData. it is additional POS's data every POS has it and it's unique

        public Nullable<int> PHYGroups_PHYGrpID { get; set; } //FK from PHYGroups could be Null because the user only fill out this info when the value choosen of dbo.FvPLists is PHY

        [Required]
        [Display(Name = "POS NAME")]
        public string PosName { get; set; }

        [Required]
        [Display(Name = "TAXID")]
        [StringLength(80)]
        public string TaxID { get; set; }

        [Required]
        [Display(Name = "DBA NAME")]
        public string DBA_Name { get; set; }

        [Required]
        [Display(Name = "ACCOUNT MANAGER")]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Name.")]
        public string POSFAC_Manager { get; set; }
    }
}