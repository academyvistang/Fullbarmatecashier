using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace BarAndRestaurantMate.Hubs
{
    public class KitchenHub : Hub
    {
        public static void Show()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<KitchenHub>();
            context.Clients.All.displayStatus();
        }

        public static void Notify()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<KitchenHub>();
            context.Clients.All.notifyStatus();
        }

        public static void Alert()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<KitchenHub>();
            context.Clients.All.alertStatus();
        }



        public static void PrinterAlert()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<KitchenHub>();
            context.Clients.All.displayPrinter();
        }
    }
}