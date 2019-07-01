using System;
using System.Net;
using System.Net.Sockets;

namespace ServerConsoleApp
{

    public struct UdpState
    {
        public UdpClient client;
        public IPEndPoint dataStream;
    }
    public class UdpSocket
    {

        private bool running;
        public UdpClient socket;
        public UdpState socketState;
        public UdpSocket(IPEndPoint target, int port) {
            socket = new UdpClient(port);

            this.socketState = new UdpState() {
                client = socket,
                dataStream = target
            };

        }
        public UdpSocket(IPEndPoint target) {
            socket = new UdpClient();

            this.socketState = new UdpState() {
                client = socket,
                dataStream = target
            };

        }
        #region public methods
        public void Receive(AsyncCallback callback) {
            socket.BeginReceive(callback, socketState);
        }
        public void Send(Message msg, AsyncCallback callback, UdpState s) {
            socket.BeginSend(msg.GetBytes(), msg.GetBytes().Length, s.dataStream, callback, s);
        }
        public void Send(Message msg, AsyncCallback callback) {
            socket.BeginSend(msg.GetBytes(), msg.GetBytes().Length, socketState.dataStream, callback, socketState);
        }
        public void Shutdown() {
            running = false;
            socket.Close();
            Environment.Exit(0);
        }

        public void ListenForMessages(AsyncCallback callback) {
            if (running) {
                Console.WriteLine("Already Running!");
                return;
            }
            try {
                Receive(callback);
                running = true;
            } catch (Exception e) {
                Console.WriteLine(e);
            }

        }
        #endregion

    }

}