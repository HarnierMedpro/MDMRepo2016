using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMCorp_Owner
    {
        public int corpOwnerID { get; set; }

        [Display(Name="CORPORATE NAME")]
        public int corpID { get; set; }

        [Display(Name="OWNER NAME")]
        public int OwnersID { get; set; }

        //public string CorporateName { get; set; }

        public bool active { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}