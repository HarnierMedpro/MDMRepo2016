using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    public class VMPosIds
    {
        public int ZoomDBPOSID { get; set; } //PK

        [Display(Name = "POS ID")]
        public long ZoomPos_ID { get; set; } //POSId

        [Display(Name = "EXTRA")]
        public string Extra { get; set; }

        [Display(Name = "POS NAME")]
        public int MasterPOSID { get; set; }

        [Display(Name = "ACTIVE")]
        public bool Active { get; set; }

        [Display(Name = "ZOOM POS_ID NAME")]
        public string ZoomPos_Name { get; set; }
    }
}