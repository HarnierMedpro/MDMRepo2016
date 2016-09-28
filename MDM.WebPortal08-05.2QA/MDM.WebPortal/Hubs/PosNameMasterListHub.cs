using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MDM.WebPortal.Hubs
{
        [HubName("MasterPOS")]
        public class PosNameMasterListHub : Hub
        {
            public static void DoIfDuplicatePosIds(string notificacion)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Hubs.PosNameMasterListHub>();
                context.Clients.All.NotifyIfDuplicatePosIDs(notificacion);
            }
        }
}