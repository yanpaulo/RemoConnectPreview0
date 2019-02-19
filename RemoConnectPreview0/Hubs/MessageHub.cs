using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoConnectPreview0.Hubs
{
    public class MessageHub : Hub
    {
        
        public async Task SwitchAsync(bool message)
        {
            await Clients.All.SendAsync("Switch", message);
        }
        
    }
}
