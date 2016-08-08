using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMCorp_Owner
    {
        public int corpOwnerID { get; set; }

        public int corpID { get; set; }

        public int OwnersID { get; set; }

        //public string CorporateName { get; set; }

        public bool active { get; set; }
    }
}