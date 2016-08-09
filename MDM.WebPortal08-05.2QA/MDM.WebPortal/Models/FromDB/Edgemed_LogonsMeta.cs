using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MDM.WebPortal.Data_Annotations;

namespace MDM.WebPortal.Models.FromDB
{
    public class Edgemed_LogonsMeta
    {
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
    [MetadataType(typeof(Edgemed_LogonsMeta))]
    public partial class Edgemed_Logons
    {
    }
}