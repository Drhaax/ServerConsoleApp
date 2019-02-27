using System;
using System.Net;
using System.Net.Sockets;

namespace ServerConsoleApp {

	public struct UdpState {
		public UdpClient client;
		public IPEndPoint dataStream;
	}
	public class UdpSocket {

		private bool running;
		public UdpClient socket;

		public UdpSocket(int port) {
			socket = new UdpClient(port);
		}
		public UdpSocket() {
			socket = new UdpClient();
		}
		#region public methods
		public void Receive(AsyncCallback callback, UdpState state) {
			socket.BeginReceive(callback, state);
		}
		public void Send(Message msg, AsyncCallback callback, IPEndPoint target, UdpState state) {
			socket.BeginSend(msg.GetBytes(), msg.GetBytes().Length, target, callback, state);
		}
		public void Shutdown() {
			running = false;
			socket.Close();
			Environment.Exit(0);
		}

		public void ListenForMessages(AsyncCallback callback, UdpState state) {
			if (running) {
				Console.WriteLine("Already Running!");
				return;
			}
			try {
				Receive(callback, state);
				running = true;
			} catch (Exception e) {
				Console.WriteLine(e);
			}

		}
		#endregion

	}

}