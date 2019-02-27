using ServerConsoleApp;
using System;
using System.Collections.Generic;
using System.Net;


namespace ServerConsoleApp {
	class Program {
		static UdpSocket Server;
		static List<Player> Players;
		static void Main(string[] args) {
			 Players = new List<Player>();

			Server = new UdpSocket(8000);
			UdpState state = new UdpState() {
				client = Server.socket,
				dataStream = new IPEndPoint(IPAddress.Any, 5330)
			};
			try {
				Server.ListenForMessages(OnReceive,state);
				while (true) {
					var input = Console.ReadLine();
					if (input == "close") {
						break;
					}
					if (input == "Debug" || input == "debug") {
						GlobalVariables.DebugMode = !GlobalVariables.DebugMode;
						Console.WriteLine("Debug mode set: " + GlobalVariables.DebugMode);
					}
				}
			} catch (Exception e) {
				Console.WriteLine(e);
				throw;
			}
		}

		public static void OnReceive(IAsyncResult ar) {
			try {
				var state = (UdpState)ar.AsyncState;
				var bytes = state.client.EndReceive(ar, ref state.dataStream);
				var msg = new Message(bytes);
				switch (msg.type) {
					case MessageType.LogIn:
						foreach (var p in Players) {
							if (p.name == msg.name) {
								var errorMsg = new Message { type = MessageType.LogIn, name = msg.name, message = "Error: " + msg.name + " is already logged in" };
								Server.Send(errorMsg, OnSend, state.dataStream,state);
								Server.Receive(OnReceive, state);
								return;
							}
						}

						Players.Add(new Player {name = msg.name});
						var msgs = new Message { type = MessageType.LogIn, name = msg.name, message = "Success", x = 0, y = 0, z = 0 };
						Server.Send(msgs, OnSend, state.dataStream,state);
						Print.Line(msg.type + " " + msg.message);
						break;
					
					case MessageType.LogOut:
						Print.Line(msg.type + " " + msg.message);
						break;
					
					case MessageType.Movement:
						foreach (var p in Players) {
							Server.Send(new Message { type = MessageType.Movement, name = p.name, x = p.x, y = p.y, z = p.z }, OnSend, state.dataStream, state);

						}
						Print.Line(msg.type + " " + msg.name + " " + msg.x + " " + msg.y + " " + msg.z);
						break;

					case MessageType.None:
						Print.Line(msg.type + " " + msg.message);
						break;
				}

				Server.Receive(OnReceive, state);

			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		private static void OnSend(IAsyncResult ar) {
		}
	}
}

public static class Print {
	public static void Line(string msg) {
		if (GlobalVariables.DebugMode) {
			Console.WriteLine(msg);
		}
	}
}