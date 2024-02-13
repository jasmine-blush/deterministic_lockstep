using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer {
    public struct ReceivedMessageOld {
        public IPEndPoint Sender;
        public string Message;
    }

    public struct ReceivedMessage {
        public IPAddress IPAddr;
        public byte[] Message;
    }

    public class UdpServer {
        private UdpClient _senderClient;
        private UdpClient _listenerClient;
        private int _clientPort;

        public UdpServer(IPEndPoint endpoint) {
            _listenerClient = new UdpClient(endpoint);
            _senderClient = new UdpClient(new IPEndPoint(endpoint.Address, endpoint.Port+1));

            //temp
            _clientPort = endpoint.Port;
        }

        public async Task<ReceivedMessage> AsyncReceiveMessageBytes() {
            var receivedMessage = await _listenerClient.ReceiveAsync();
            return new ReceivedMessage() {
                IPAddr = receivedMessage.RemoteEndPoint.Address,
                Message = receivedMessage.Buffer
            };
        }

        public void SendMessage(byte message, IPAddress address) {
            _senderClient.Send(new byte[] { message }, 1, new IPEndPoint(address, _clientPort));
        }

        public void SendMessage(byte[] message, IPAddress address) {
            _senderClient.Send(message, message.Length, new IPEndPoint(address, _clientPort));
        }

        public void CloseServer() {
            _senderClient.Close();
            _listenerClient.Close();
        }
    }
}
