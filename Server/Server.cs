// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace MyServer
{
    public class Server
    {
        private Socket serverSock;
        public  static Action<String> log;
        public Server(Action<String> log)
        {
            Server.log = log;

        }

        public void Run()
        {
            serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            serverSock.Bind(endPoint);
            serverSock.Listen(0);
            Server.log("Server started on port 8000");
            // main loop to accept connections
            while (true)
            {
                Socket client = serverSock.Accept();
                // here you start a new thread.
                ClientHandler cHandler = new ClientHandler(client);
                // new thread for every client
                Task.Run(() =>
                {
                    cHandler.Run();
                });
            }
        }
    }
}
