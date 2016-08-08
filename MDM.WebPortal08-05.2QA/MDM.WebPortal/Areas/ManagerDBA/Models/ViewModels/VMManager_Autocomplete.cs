using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.ManagerDBA.Models.ViewModels
{
    public class VMManager_Autocomplete
    {
        public string ManagerID { get; set; }

        public string AliasName { get; set; }

        public bool Active { get; set; }
    }
}