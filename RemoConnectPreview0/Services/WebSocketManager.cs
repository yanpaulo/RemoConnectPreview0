using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoConnectPreview0.Services
{
    public class WebSocketManager
    {
        private List<WebSocket> webSockets = new List<WebSocket>();
        public void RegisterWebSocket(WebSocket webSocket)
        {
            DoCleanup();
            webSockets.Add(webSocket);
            webSocket.ReceiveAsync(new Memory<byte>(), CancellationToken.None).AsTask().Wait();
        }

        public async Task SendAsync(string message)
        {
            DoCleanup();
            var tasks = webSockets.Select(s => s.SendAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None).AsTask());
            await Task.WhenAll(tasks);
        }


        private void DoCleanup()
        {
            webSockets.RemoveAll(s => s.CloseStatus.HasValue);
        }
    }
}
