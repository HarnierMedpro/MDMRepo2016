using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels
{
    public class VMLocPOS_Contact
    {
        public int Facility_DBs_IDPK { get; set; }

       
        [Display(Name = "CONTACTS")]
        [UIHint("ContactsEditor")]
        public IEnumerable<VMContactInfo> Contacts { get; set; }

        public VMLocPOS_Contact()
        {
            Contacts = new List<VMContactInfo>();
        }
    }
}