using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMCorp_Contact
    {
        public int corpID { get; set; }

       
        [Display(Name = "CONTACTS")]
        [UIHint("CorpContactEditor")]
        public IEnumerable<VMContact> Contacts { get; set; }

        public VMCorp_Contact()
        {
            Contacts = new List<VMContact>();
        }
    }
}