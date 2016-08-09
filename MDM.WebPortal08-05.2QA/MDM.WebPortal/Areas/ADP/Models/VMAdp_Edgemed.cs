using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MDM.WebPortal.Data_Annotations;

namespace MDM.WebPortal.Areas.ADP.Models
{
    public class VMAdp_Edgemed
    {
        [Display(Name = "EMPLOYEE")]
        public int ADPMasterID { get; set; }

        [Required]
        [Display(Name = "FIRST NAME")]
        [StringLength(50)]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid First Name.")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "LAST NAME")]
        [StringLength(50)]
        [RegularExpression(@"^([a-zA-Z \\&\'\-]+)$", ErrorMessage = "Invalid Last Name.")]
        public string LName { get; set; }

        public int Edgemed_LogID { get; set; }

        [Required]
        [Display(Name = "EDGEMED USERNAME")]
        [StringLength(50)]
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