using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ADP.Models
{
    public class VMEdgemed_Logons
    {
        //PK
        public int Edgemed_LogID { get; set; }

        //FK from dbo.ADPMaster table
        public int ADPMasterID { get; set; }

        [Display(Name = "EDGEMED USERNAME")]
        [StringLength(50)]
        public string Edgemed_UserName { get; set; }

        [Display(Name = "ZOOM SERVER")]
        public int Zno { get; set; }

        [Display(Name = "EDGEMED ID")]
        public int EdgeMed_ID { get; set; }

        [Display(Name = "ACTIVE")]
        [Required]
        public bool Active { get; set; }
    }
}