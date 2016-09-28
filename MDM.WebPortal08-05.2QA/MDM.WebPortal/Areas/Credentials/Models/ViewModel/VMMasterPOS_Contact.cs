using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMMasterPOS_Contact
    {
        public int MasterPOSContactID { get; set; } //PK

        [Display(Name = "CONTACTS")]
        [UIHint("AvailablePOSContactEditor")]
        public IEnumerable<VMContact> Contacts { get; set; }

        [Display(Name = "POS")]
        public int MasterPOS_MasterPOSID { get; set; }
    }
}