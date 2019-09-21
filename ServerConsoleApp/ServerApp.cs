using ServerConsoleApp;
using System;
using System.Collections.Generic;
using System.Net;


namespace ServerConsoleApp {
	class Program {
		static UdpService Server;
		static List<Player> Players;
		static void Main(string[] args) {
			Players = new List<Player>();
            Server = new UdpService(8000,8000);
            Server.AddMessageCallback(OnLogin, OnLogout, OnMovement);
			
            try {
				Server.StartService();
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
        static void OnLogin(Message msg, UdpState s) {
            foreach (var p in Players) {
                if (p.name == msg.name) {
                    var errorMsg = new Message { type = MessageType.LogIn, name = msg.name, message = "Error: " + msg.name + " is already logged in" };
                    Server.SendMessage(errorMsg,s);
                    return;
                }
            }

            Players.Add(new Player { name = msg.name });
            var msgs = new Message { type = MessageType.LogIn, name = msg.name, message = "Success", x = 0, y = 0, z = 0 };
            Server.SendMessage(msgs, s);
            Print.Line(msg.type + " " + msg.message);
        }

        static void OnLogout(Message msg, UdpState s) { 
        }

        static void OnMovement(Message msg, UdpState s) {
            var msgs = new Message {
                type = MessageType.Movement,
                x = msg.x,
                y = msg.y,
                z = msg.z
            };
            Server.SendMessage(msgs, s);

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
    public static void ErrorLine(string msg) {
        if (GlobalVariables.DebugMode) {
            Console.WriteLine("Error: "+ msg);
        }
    }
}