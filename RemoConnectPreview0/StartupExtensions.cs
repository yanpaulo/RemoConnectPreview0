using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RemoConnectPreview0.Services;

namespace RemoConnectPreview0
{
    public static class StartupExtensions
    {
        public static IApplicationBuilder UseWebSocketService(this IApplicationBuilder app, string url = "/ws") => app.Use(async (context, next) =>
        {
            if (context.Request.Path == url)
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    app.ApplicationServices.GetService<Services.WebSocketManager>().RegisterWebSocket(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next();
            }

        });
    }
}
