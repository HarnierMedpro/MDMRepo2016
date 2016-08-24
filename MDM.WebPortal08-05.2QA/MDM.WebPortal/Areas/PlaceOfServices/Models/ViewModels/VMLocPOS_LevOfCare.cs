using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMLocPOS_LevOfCare
    {
        public int LocPosLocID { get; set; }

        [Display(Name = "LEVEL OF CARE")]
        public int Lev_of_Care_LevOfCareID { get; set; }

        [Display(Name = "LOCATION POS")]
        public int LocationsPOS_Facitity_DBs_IDPK { get; set; }
    }
}