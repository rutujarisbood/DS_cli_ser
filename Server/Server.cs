// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Collections.Specialized;
using System.Threading;

namespace MyServer
{
    public class Server
    {
        private Socket serverSock;
        private Socket backupServer;
        public static Action<String> log;
        private int portNo;
        private static Socket sock;

        public static ObservableCollection<string> messagesForBackup = new ObservableCollection<string>();
        public static bool isPrimary;

        public Server(Action<String> log, bool isPrimary)
        {
            Server.log = log;
            Server.isPrimary = isPrimary;
            portNo = int.Parse(ConfigurationManager.AppSettings.Get(isPrimary ? "PrimaryPort" : "BackupPort"));
        }



        public void Run()
        {
            initSocket(portNo);
            Server.log("Server started on port : " + portNo);

            if (isPrimary)
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(IPAddress.Parse("127.0.0.1"), 8001);
                messagesForBackup.CollectionChanged += ChangeEvent;
            }

            bool ranOnce = false;
            bool ranTwice = false;
            while (true)
            {
                if (!isPrimary && ranOnce && !ranTwice)
                {
                    TryUntil(()=> initSocket(int.Parse(ConfigurationManager.AppSettings.Get("PrimaryPort"))));
                    ranTwice = true;
                }
                Socket client = serverSock.Accept();
                // here we start a new thread.
                ClientHandler cHandler = new ClientHandler(client);
                // new thread for every client
                Task.Run(() =>
                {
                    cHandler.Run();
                });
                ranOnce = true;
            }
        }

        private void TryUntil(Action p)
        {
            for(; ; )
            {
                try
                {
                    p.Invoke();
                }
                catch
                {
                    Thread.Sleep(200);
                    // try until successfull
                    continue;
                }
                // break on success
                break;
            }
        }

        private void initSocket(int portNo)
        {
            serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNo);
            serverSock.Bind(endPoint);
            serverSock.Listen(0);
        }

        private static void ChangeEvent(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var message = string.Join(" ", messagesForBackup);
                sock.SendMessage(new Dictionary<string, string> { { "updateData", "yes" }, { "words", message } });
                messagesForBackup.Clear();
            }
        }
    }
}
