using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MDM.WebPortal.Data_Annotations;

namespace MDM.WebPortal.Areas.ADP.Models
{
    public class VMEdgemed_Logons
    {
        //PK
        public int Edgemed_LogID { get; set; }

        //FK from dbo.ADPMaster table
        [Display(Name = "EMPLOYEE")]
        public int ADPMasterID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "EDGEMED USERNAME")]
        public string Edgemed_UserName { get; set; }

        [Required]
        [Display(Name = "ZOOM SERVER")]
        [BiggerThanCero(ErrorMessage = "The Zoom Server value needs to be bigger than zero.")]
        public int Zno { get; set; }

        [Required]
        [Display(Name = "EDGEMED ID")]
        [BiggerThanCero(ErrorMessage = "The EdgeMed_ID value needs to be bigger than zero.")]
        public int EdgeMed_ID { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }
    }
}