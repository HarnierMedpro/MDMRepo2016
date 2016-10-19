using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MDM.WebPortal.Hubs
{
    [HubName("UpdatePOS")]
    public class UpdatePosInCorHub : Hub
    {
        public static void DoIfCreateNewPos(int corpID)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Hubs.UpdatePosInCorHub>();
            context.Clients.All.NotifyIfCreateNewPOS(corpID);
        }
    }
}