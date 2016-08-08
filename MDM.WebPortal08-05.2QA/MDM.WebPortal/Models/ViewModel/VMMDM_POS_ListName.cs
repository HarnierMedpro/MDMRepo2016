using System;
using System.ComponentModel.DataAnnotations;


namespace MDM.WebPortal.Models.ViewModel
{
    public class VMMDM_POS_ListName
    {
        public int MDMPOS_ListNameID { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "POS NAME")]
        public string PosName { get; set; }

        [Required]
        public Nullable<bool> active { get; set; }
    }

   
}