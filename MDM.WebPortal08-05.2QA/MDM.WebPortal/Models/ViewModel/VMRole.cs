using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMRole
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int Priotity { get; set; }
    }
}