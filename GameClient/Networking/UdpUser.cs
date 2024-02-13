using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.Networking {
    public sealed class UdpUser {
        private UdpClient _senderClient;
        private UdpClient _listenerClient;
        private readonly IPEndPoint _serverEndPoint;

        public UdpUser(IPEndPoint endpoint) {
            /*!!Debug!! goes through possible local IPs and picks a free one*/
            int ip = 98;
            while(_listenerClient == null && ip < 255) {
                try {
                    _listenerClient = new UdpClient(new IPEndPoint(IPAddress.Parse("192.168.0." + ip), 48620));
                    _senderClient = new UdpClient(new IPEndPoint(IPAddress.Parse("192.168.0." + ip), 48621));
                } catch(Exception e) {
                    Console.WriteLine(e);
                    ip++;
                }
            }
            Console.WriteLine("192.168.0." + ip);

            _serverEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.97"), endpoint.Port);  //Server on this IP and same port as client
        }

        public void SendMessage(byte message) {
            _senderClient.Send(new byte[] { message }, 1, _serverEndPoint);
        }

        public void SendMessage(byte[] message) {
            _senderClient.Send(message, message.Length, _serverEndPoint);
        }

        public async Task<byte[]> AsyncReceiveMessage() {
            var result = await _listenerClient.ReceiveAsync();
            return result.Buffer;
        }

        public void Close() {
            _senderClient.Close();
            _listenerClient.Close();
        }
    }
}
