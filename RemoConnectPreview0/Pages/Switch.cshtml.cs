using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RemoConnectPreview0.Hubs;
using RemoConnectPreview0.Services;

namespace RemoConnectPreview0.Pages
{
    public class SwitchModel : PageModel
    {
        private readonly WebSocketManager _socketManager;

        public SwitchModel(WebSocketManager socketManager)
        {
            _socketManager = socketManager;
        }

        [BindProperty]
        public string Action { get; set; }

        public async Task OnPostAsync()
        {
            var value = Action?.ToLower();
            switch (value)
            {
                case "on":
                case "off":
                    await _socketManager.SendAsync(value);
                    break;
            }
        }
    }
}