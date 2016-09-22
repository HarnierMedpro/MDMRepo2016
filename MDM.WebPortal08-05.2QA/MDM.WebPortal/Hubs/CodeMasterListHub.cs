using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MDM.WebPortal.Hubs
{
    [HubName("CodeMaster")]
    public class CodeMasterListHub :  Hub
    {
        public static void DoIfCodeCreated(int CodeID, string Code)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<CodeMasterListHub>();
            context.Clients.All.NotifyIfCreated(CodeID, Code);
        }
    }
}