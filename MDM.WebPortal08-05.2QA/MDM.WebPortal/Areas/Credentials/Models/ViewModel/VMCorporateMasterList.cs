using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.Credentials.Models.ViewModel
{
    public class VMCorporateMasterList
    {
        public int corpID { get; set; }

        [Required]
        [Display(Name = "CORPORATION")]
        public string CorporateName { get; set; }

        [Required]
        [Display(Name = "ACTIVE")]
        public bool active { get; set; }

        [Required]
        [Display(Name = "TAXID")]
        public string TaxID { get; set; }

        [Display(Name = "OWNERS")]
        [UIHint("OwnersEditor")]
        public IEnumerable<VMContact> Owners { get; set; }

        //[Display(Name = "CONTACTS")]
        //[UIHint("CorpContactEditor")]
        //public IEnumerable<VMContact> Contacts { get; set; }

        [Required]
        [Display(Name = "DBs")]
        [UIHint("CorpDBEditor")]
        public IEnumerable<VMDBList> DBs { get; set; }

        public VMCorporateMasterList()
        {
            Owners = new List<VMContact>();
            DBs = new List<VMDBList>();
        }
    }
}