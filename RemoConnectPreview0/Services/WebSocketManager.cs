using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemoConnectPreview0.Services.Communication;
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
        private Dictionary<string, WebSocket> webSockets = new Dictionary<string, WebSocket>();
        private readonly Dictionary<DeviceOperation, Type> operationDataTypes = new Dictionary<DeviceOperation, Type>
        {
            { DeviceOperation.Register, typeof(RegisterData) }
        };

        public async Task RegisterWebSocketAsync(WebSocket webSocket)
        {
            if (await HandleNextAsync(webSocket) != DeviceOperation.Register)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Are you crazy?", CancellationToken.None);
            }

            while (webSocket.State == WebSocketState.Open)
            {
                await HandleNextAsync(webSocket); 
            }
        }

        public async Task SendAsync(string message)
        {
            Cleanup();
            var tasks = webSockets.Select(s => s.Value.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None));
            await Task.WhenAll(tasks);
        }

        private async Task<DeviceOperation> HandleNextAsync(WebSocket socket)
        {
            var message = await ReceiveMessageAsync(socket);

            switch (message.Data)
            {
                case RegisterData register:
                    RegisterDevice(socket, register);
                    break;
                default:
                    break;
            }

            return message.Operation;
        }

        private void RegisterDevice(WebSocket socket, RegisterData data)
        {
            if (webSockets.TryGetValue(data.DeviceId, out WebSocket old))
            {
                webSockets.Remove(data.DeviceId);
            }
            webSockets.Add(data.DeviceId, socket);
        }

        

        private async Task<DeviceMessage> ReceiveMessageAsync(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.Count > 0)
            {
                var content = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var message = JsonConvert.DeserializeObject<DeviceMessage>(content);
                var jObject = message.Data as JObject;
                message.Data = jObject.ToObject(operationDataTypes[message.Operation]);
                return message;
            }
            throw new InvalidOperationException();
        }
        
        private void Cleanup()
        {
            foreach (var s in webSockets.Where(v => v.Value.State != WebSocketState.Open).ToList())
            {
                webSockets.Remove(s.Key);
            }
        }
    }
}
