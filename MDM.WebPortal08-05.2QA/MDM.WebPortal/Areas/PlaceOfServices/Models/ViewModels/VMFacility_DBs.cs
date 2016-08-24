using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MDM.WebPortal.Data_Annotations;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMFacility_DBs
    {
        public int IDPK { get; set; }

        [Required]
        [Display(Name = "DB")]
        public string DB { get; set; }

        [Display(Name = "DATABASE NAME")]
        public string DatabaseName { get; set; }

        [Required]
        [Display(Name = "FACILITY ID")]
        //[BiggerThanCero(ErrorMessage = "The value of this field needs to be bigger than zero.")]
        public long Facility_ID { get; set; }

        [Required]
        [Display(Name = "FACILITY NAME")]
        public string Fac_NAME { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }
    }
}