using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MDM.WebPortal.Hubs
{
       [HubName("CorporateMaster")]
        public class CorporateMasterListHub : Hub
        {
            public static void DoIfDBDuplicated(string notificacion)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Hubs.CorporateMasterListHub>();
                context.Clients.All.NotifyIfDuplicated(notificacion);
            }
        }
    
}