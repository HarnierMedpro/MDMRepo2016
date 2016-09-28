using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMForms_Sent
    {
        public int FormSentID { get; set; }

        [Display(Name = "FORMS")]
        public int FormsDict_FormsID { get; set; }

        [Display(Name = "POS")]
        public int MasterPOS_MasterPOSID { get; set; }
    }
}