using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMLocPOS_MPServ
    {
        public int LocPosMPServID { get; set; }

        [Display(Name = "SERVICE")]
        public int MPServices_MPServID { get; set; }

        [Display(Name = "LOCATION POS")]
        public int LocationsPOS_Facitity_DBs_IDPK { get; set; }
    }
}