
// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943

// https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-C-Sharp/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public static class Client
    {
        private static Socket sock;
        public static string fileToSend = null;

        // https://docs.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1?view=net-5.0
        //  an event is fired whenever any modification happens to a collection.
        // Since we are not using a Queue, we clear the contents of wordsToAddToLexicon after we use it to send to server which results in event trigger.
        public static ObservableCollection<string> wordsToAddToLexicon = new ObservableCollection<string>();

        private static Action<String> ClientLog;
        public static Action<String> updateLexicons;
        static Client()
        {
            // we set our event handler in the static constructor of this class.
            wordsToAddToLexicon.CollectionChanged += WordsToAddToLexicon_CollectionChanged;
        }

        private static void WordsToAddToLexicon_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Instead of the append, we simply set the final text of the lexicon box.
            // If there are values in the list, set val, else set null when the list is cleared (When words are sent to server.)
            string val = String.Join(Environment.NewLine, wordsToAddToLexicon);
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                updateLexicons(val);
            else updateLexicons(null);

        }

        private static void SendMessageToServer(Dictionary<string, string> message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(new NetworkStream(sock)))
                {
                    var x = JsonConvert.SerializeObject(message);
                    writer.WriteLine(JsonConvert.SerializeObject(message));
                    writer.Flush();
                    //wordsToAddToLexicon.Clear();
                }
            }
            catch (Exception e)
            {
                ClientLog("Could not connect.Server connection lost.");
            }
        }

        private static Dictionary<string, string> ReadMessage()
        {

            try
            {
                // https://stackoverflow.com/questions/1450263/c-sharp-streamreader-readline-does-not-work-properly

                using (StreamReader reader = new StreamReader(new NetworkStream(sock)))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        return JsonConvert.DeserializeObject<Dictionary<string, string>>(line);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void SendFile(String filename, String clientName)
        {
            Dictionary<string, string> message = null;
            if (filename != null)
            {
                message = new Dictionary<string, string> { { "username", clientName }, { "file", File.ReadAllText(filename) } };
                SendMessageToServer(message);
            }
        }

        public static void exitClient(String clientName)
        {
            var fmessage = new Dictionary<string, string> { { "exit", clientName } };
            SendMessageToServer(fmessage);
        }

        public static void addWordToQueue(String wordToAdd)
        {
            ClientLog("word added to queue");
            wordsToAddToLexicon.Add(wordToAdd);
        }

        public static void Run(Action<String> log, String clientName)
        {
            try
            {
                ClientLog = log;
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(IPAddress.Parse("127.0.0.1"), 8000);

                log("Connected to server.");
                var fmessage = new Dictionary<string, string> { { "register", clientName } };
                SendMessageToServer(fmessage);
                while (true)
                {
                    while ((fmessage = ReadMessage()) == null) { }
                    // wait until a reply

                    if (fmessage["status"] == "Accepted")
                    {
                        log("Username accepted.");
                        break;
                    }
                    else
                    {
                        log("Username rejected.");
                        break;
                    }
                }

                while (true)
                {
                    Dictionary<string, string> message = null;
                    while ((message = ReadMessage()) != null)
                    {
                        if (message.ContainsKey("status") && message.ContainsKey("file"))
                        {
                            switch (message["status"])
                            {
                                case "Done":
                                    log("File processing complete");
                                    log("File contents: " + message["file"]);
                                    break;
                                case "Failed":
                                    log("File processing failed.");
                                    break;
                            }
                        }
                        else if (message.ContainsKey("poll") && message["poll"] == "checkQueue" && wordsToAddToLexicon.Count > 0)
                        {
                            ClientLog("preparing lexicon queue to send");

                            // get the contents of list separated by space.
                            string result = String.Join(' ', wordsToAddToLexicon);
                            wordsToAddToLexicon.Clear();

                            ClientLog("sending to server : " + result);
                            var frmessage = new Dictionary<string, string> { { "wordQueue", result } };
                            SendMessageToServer(frmessage);
                        }

                    }


                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
