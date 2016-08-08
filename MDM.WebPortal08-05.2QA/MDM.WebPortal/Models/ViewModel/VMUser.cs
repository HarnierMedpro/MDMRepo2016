using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.ViewModel
{
    public class VMUser
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public bool Active { get; set; }
        public string roleId { get; set; }
    }
}