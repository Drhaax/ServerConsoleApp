using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using ServerConsoleApp;

public class UdpService
{
    private UdpSocket client;

    public UdpService(int targetPort, string targetHost) {
        //for client
        client = new UdpSocket(new IPEndPoint(IPAddress.Parse(targetHost), targetPort));
    }
    public UdpService(int bindPort, int targetPort) {
        //for serverside
        client = new UdpSocket(new IPEndPoint(IPAddress.Any, targetPort), bindPort);
    }
    internal void StartService() {
        client.ListenForMessages(OnReceive);
    }

    private void OnSend(IAsyncResult ar) {
        var aaa = (UdpState)ar.AsyncState;
        Print.Line("Sent " + aaa.dataStream.ToString());

    }

    void OnReceive(IAsyncResult ar) {
        var s = (UdpState)ar.AsyncState;
        Print.Line("receive" + s.dataStream.ToString());
        var bytes = s.client.EndReceive(ar, ref s.dataStream);
        var msg = new Message(bytes);

        switch (msg.type) {
            case MessageType.None:
            break;
            case MessageType.LogIn:
            if (Login != null) {
                Login.Invoke(msg, s);
            }
            break;
            case MessageType.LogOut:
            if (Logout != null) {
                Logout.Invoke(msg, s);
            }
            break;
            case MessageType.Movement:
            if (Movement != null) {
                Movement.Invoke(msg, s);
            }
            break;
        }
        client.Receive(OnReceive);
    }
    ///<summary>Send message to specific "client" using UdpState</summary>
    public void SendMessage(Message msg, UdpState s) {
        client.Send(msg, OnSend, s);
    }
    ///<summary>Send message to "default client" without specifying UdpState</summary>
    public void SendMessage(Message msg) {
        client.Send(msg, OnSend);
    }
    Action<Message, UdpState> Login;
    Action<Message, UdpState> Logout;
    Action<Message, UdpState> Movement;
    public void AddMessageCallback(Action<Message, UdpState> Login, Action<Message, UdpState> Logout, Action<Message, UdpState> Movement) {
        this.Login = Login;
        this.Logout = Logout;
        this.Movement = Movement;
    }
}
