using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMMasterPOSPartial
    {
         public int MasterPOSID { get; set; }
         public string PosMasterName { get; set; }
         public bool active { get; set; }
    }
}