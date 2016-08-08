using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace MDM.WebPortal.App_Start
{
    public static class UtilClass
    {
        public static string myuser;

        public static string currentNTUser()
        {
            myuser = WindowsIdentity.GetCurrent().Name;
            return myuser;
        }
    }
}