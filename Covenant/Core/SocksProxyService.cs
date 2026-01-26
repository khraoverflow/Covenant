using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Covenant.Core
{
    public class SocksProxyService : BackgroundService
    {
        private readonly IWebsocketService _websocketService;

        private TcpListener _listener;
        private readonly ConcurrentDictionary<int, TunnelConnectionState> _eventWaiters = new();
        private ConcurrentDictionary<int, TcpClient> _listeners = new ConcurrentDictionary<int, TcpClient>();

        private readonly string _userName;
        private readonly string _password;

        private int _nextClientId = 1;
        private readonly object _lock = new();

        public SocksProxyService(IWebsocketService websocketService, IPAddress ip, string port, string userName, string password)
        {
            _websocketService = websocketService;
            var _socksPort = int.Parse(port);
            _listener = new TcpListener(ip, _socksPort);
            _userName = userName;
            _password = password;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _websocketService.DataReceived += DataRceivednHandler;
            _websocketService.MessageReceived += MessageReceivedHandler;

            _listener.Start();

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    TcpClient client;

                    try
                    {
                        client = await _listener.AcceptTcpClientAsync(stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    _ = HandleTcpConnection(client, stoppingToken);
                }
            }
            finally
            {
                _listener.Stop();

                _websocketService.DataReceived -= DataRceivednHandler;
                _websocketService.MessageReceived -= MessageReceivedHandler;
            }
        }

        private async Task HandleTcpConnection(TcpClient client, CancellationToken cancellationToken)
        {
            using NetworkStream clientStream = client.GetStream();

            //1.Greeting
            byte[] greet = new byte[2];
            await clientStream.ReadAsync(greet);
            int methods = greet[1];
            byte[] methodsBuf = new byte[methods];
            await clientStream.ReadAsync(methodsBuf);

            if (string.IsNullOrWhiteSpace(_userName) ||
                string.IsNullOrWhiteSpace(_password))
            {
                // Reply: no authentication
                await clientStream.WriteAsync(new byte[] { 0x05, 0x00 });
            }
            else
            {
                await ValidateAuth(clientStream);
            }

            // 2. Connect request
            byte[] header = new byte[4];
            await clientStream.ReadAsync(header);
            if (header[1] != 0x01) // only CONNECT supported
                return;

            string _host;
            int _port;
            byte atyp = header[3];
            
            if (atyp == 0x01) // IPv4
            {
                byte[] addr = new byte[4];
                await clientStream.ReadAsync(addr);
                _host = new IPAddress(addr).ToString();
            }
            else if (atyp == 0x03) // Domain
            {
                int len = clientStream.ReadByte();
                byte[] addr = new byte[len];
                await clientStream.ReadAsync(addr);
                _host = Encoding.ASCII.GetString(addr);
            }
            else
                return;

            byte[] portBuf = new byte[2];
            await clientStream.ReadAsync(portBuf);
            _port = (portBuf[0] << 8) | portBuf[1];

            // add to dict
            int id = GenerateClientId();
            _listeners[id] = client;

            var state = new TunnelConnectionState();
            _eventWaiters[id] = state;

            await SendDestinationAddress(id, _host, _port);

            var connectionOpenSuccess = await _eventWaiters[id].ConnectionEstablishedTaskCompletion.Task;
            if (!connectionOpenSuccess)
                return;

            // send back success result 
            byte[] reply = { 0x05, 0x00, 0x00, 0x01, 0, 0, 0, 0, 0, 0 };
            await clientStream.WriteAsync(reply);

            await PumpThroughWebsocket(id, clientStream, cancellationToken);

            await _eventWaiters[id].ConnectionClosedTaskCompletion.Task;

            CleanUp(id);
        }

        private void MessageReceivedHandler(object sender, byte[] e)
        {
            var responseString = Encoding.UTF8.GetString(e);
            var packet = JsonConvert.DeserializeObject<ControlPacket>(responseString);

            try
            {
                if (packet.ConnectionType == ConnectionTypes.ConnectionEstablished.ToString())
                {
                    _eventWaiters[packet.Id].ConnectionEstablishedTaskCompletion.TrySetResult(true);
                }
                if (packet.ConnectionType == ConnectionTypes.ConnectionFailed.ToString())
                {
                    _eventWaiters[packet.Id].ConnectionEstablishedTaskCompletion.TrySetResult(false);
                }
                if (packet.ConnectionType == ConnectionTypes.ConnectionClosed.ToString())
                {
                    _eventWaiters[packet.Id].ConnectionClosedTaskCompletion.TrySetResult(true);
                }
            }
            catch (Exception)
            {
            }
        }

        private async void DataRceivednHandler(object sender, byte[] e)
        {
            var packet = Decode(e);

            var inList = _listeners.TryGetValue(packet.id, out TcpClient tcpClient);
            if (!inList)
                return;

            try
            {
                var clientStream = tcpClient.GetStream();
                await clientStream.WriteAsync(packet.data);
            }
            catch (IOException)
            {
                await SendCloseSocket(packet.id);
            }
        }

        private async Task<bool> SendCloseSocket(int id)
        {
            ControlPacket bridgePacket = new ControlPacket
            {
                Id = id,
                ConnectionType = ConnectionTypes.CloseConnection.ToString()
            };

            var message = JsonConvert.SerializeObject(bridgePacket);
            var buffer = Encoding.UTF8.GetBytes(message);

            await _websocketService.SendMessage(buffer, buffer.Length);

            return true;
        }

        
        private void CleanUp(int id)
        {
            _eventWaiters.Remove(id, out _);

            try
            {
                _listeners[id].Dispose();
            }
            catch (Exception)
            {
                throw;
            }

            _listeners.Remove(id, out _);
        }

        private async Task ValidateAuth(NetworkStream clientStream)
        {
            await clientStream.WriteAsync(new byte[] { 0x05, 0x02 }, 0, 2);

            int authVer = clientStream.ReadByte(); 
            if (authVer != 0x01)
                return;

            int ulen = clientStream.ReadByte();
            byte[] uBytes = new byte[ulen];
            await clientStream.ReadAsync(uBytes, 0, ulen);

            int plen = clientStream.ReadByte();
            byte[] pBytes = new byte[plen];
            await clientStream.ReadAsync(pBytes, 0, plen);

            string username = Encoding.ASCII.GetString(uBytes);
            string password = Encoding.ASCII.GetString(pBytes);

            bool authOk = username == _userName && password == _password;

            await clientStream.WriteAsync(new byte[]
            {
                0x01,
                authOk ? (byte)0x00 : (byte)0x01
            }, 0, 2);

            if (!authOk)
                return;

        }

        private async Task SendDestinationAddress(int id, string destAddress, int destPort)
        {
            ControlPacket bridgePacket = new ControlPacket
            {
                Host = destAddress,
                Port = destPort,
                Id = id,
                ConnectionType = ConnectionTypes.OpenConnection.ToString()

            };

            var message = JsonConvert.SerializeObject(bridgePacket);
            //var message = destAddress + ":" + destPort;
            var buffer = Encoding.UTF8.GetBytes(message);

           await _websocketService.SendMessage(buffer, buffer.Length);
        }

        private async Task PumpThroughWebsocket(int id, NetworkStream clientStream, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[8192];
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {

                    var n = await clientStream.ReadAsync(buffer, 0, buffer.Length);

                    if (n == 0)
                    {
                        break;
                    }
                    var toSend = Encode(id, buffer);
                    await _websocketService.PumpThroughWebsocket(toSend, n + 4);
                }
            }
            catch (IOException ex) when (ex.InnerException is SocketException)
            {

            }
            catch (SocketException)
            {
                
            }
            finally
            {
                await SendCloseSocket(id);
            }
        }

        static byte[] Encode(int id, byte[] data)
        {
            byte[] packet = new byte[4 + data.Length];

            int i = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(id), 0, packet, i, 4);
            i += 4;

            Buffer.BlockCopy(data, 0, packet, i, data.Length);

            return packet;
        }

        static (int id, byte[] data) Decode(byte[] packet)
        {
            int i = 0;

            int id = BitConverter.ToInt32(packet, i);
            i += 4;

            byte[] data = new byte[packet.Length - i];
            Buffer.BlockCopy(packet, i, data, 0, data.Length);

            return (id, data);
        }

        public int GenerateClientId()
        {
            return Interlocked.Increment(ref _nextClientId);
        }

    }
    public class ControlPacket
    {
        public string ConnectionType { get; set; }

        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    class DecodedPacket
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public byte[] Data { get; set; }
    }

    enum ConnectionTypes
    {
        OpenConnection,
        CloseConnection,
        ConnectionClosed,
        ConnectionEstablished,
        ConnectionFailed,
    }

    class TunnelConnectionState
    {
        public TaskCompletionSource<bool> ConnectionEstablishedTaskCompletion { get; } = 
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> ConnectionClosedTaskCompletion { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
