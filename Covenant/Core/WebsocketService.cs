using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Covenant.Core
{
    public interface IWebsocketService
    {
        event EventHandler<byte[]> DataReceived;
        event EventHandler<byte[]> MessageReceived;

        Task StartListening(WebSocket ws);
        Task PumpThroughWebsocket(byte[] buffer, int length);
        Task SendMessage(byte[] buffer, int length);

    }
    public class WebsocketService : IWebsocketService
    {
        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<byte[]> MessageReceived;

        private WebSocket WebSocket;
        private byte[] buffer = new byte[8196];

        public async Task StartListening(WebSocket ws)
        {
            WebSocket = ws;
            while (WebSocket.State == WebSocketState.Open)
            {
                var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close) break;

                byte[] payload = new byte[result.Count];
                Array.Copy(buffer, payload, result.Count);
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    DataReceived?.Invoke(this, payload);
                }
                else
                {
                    MessageReceived?.Invoke(this, payload);
                }
            }
        }

        public async Task PumpThroughWebsocket(byte[] data, int length)
        {
            if (WebSocket is null || WebSocket.State != WebSocketState.Open)
                return;
            await WebSocket.SendAsync(new ArraySegment<byte>(data, 0, length),
                         WebSocketMessageType.Binary,
                         true,
                         CancellationToken.None);
        }

        public async Task SendMessage(byte[] data, int length)
        {
            if (WebSocket is null || WebSocket.State != WebSocketState.Open)
                return;
            await WebSocket.SendAsync(new ArraySegment<byte>(data, 0, length),
                         WebSocketMessageType.Text,
                         true,
                         CancellationToken.None);
        }
    }
}
