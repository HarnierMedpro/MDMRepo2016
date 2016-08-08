using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class ManagerDBAccessBIViewModel
    {
        public int id { get; set; }
        public string FvP { get; set; }
        public string DB { get; set; }
        public string Manager { get; set; }
        public string AliasName { get; set; }
        public string NTUser { get; set; }
        public Nullable<bool> active { get; set; }
    }
}