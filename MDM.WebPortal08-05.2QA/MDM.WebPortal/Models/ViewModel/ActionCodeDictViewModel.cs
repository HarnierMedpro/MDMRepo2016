using MDM.WebPortal.Models.FromDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class ActionCodeDictViewModel
    {
        public int id { get; set; }
        public string CollNoteType { get; set; }
        public string Code { get; set; }
        public int CategoryID { get; set; }
        public string Priority { get; set; }
        public string NTUser { get; set; }
        public bool Active { get; set; }
        public string ParsingYN { get; set; }

        public virtual Category Category { get; set; }
    }
}