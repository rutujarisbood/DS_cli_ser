
// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943

// https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-C-Sharp/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public static Queue<string> wordsToAddToLexicon = new Queue<string>();
        private static Action<String> ClientLog;


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
            }catch(Exception e)
            {
                ClientLog("Server connection lost.Please connect to server and try again");
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
            //byte[] byData = System.Text.Encoding.ASCII.GetBytes(wordToAdd);
            //try
            //{
            //    sock.Send(byData);
            //}
            //catch(Exception e)
            //{
            //    ClientLog(e.ToString());
            //}
            wordsToAddToLexicon.Enqueue(wordToAdd);
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
                        }else if (message.ContainsKey("poll") && message["poll"] == "checkQueue" && wordsToAddToLexicon.Count>0)
                        {
                            //ref: https://www.dotnetperls.com/convert-list-string
                            ClientLog("preparing lexicon queue to send" );
                            StringBuilder builder = new StringBuilder();
                            // Loop through all strings.
                            var temp = wordsToAddToLexicon.ToArray();
                            
                            foreach (string t in temp)
                            {
                                // Append string to StringBuilder.
                                builder.Append(" ").Append(t);
                            }
                            // Get string from StringBuilder.
                            string result = builder.ToString();
                            //Dictionary<string, string> messageToSend = null;
                            ClientLog("sending to server : " + result);
                            //messageToSend = new Dictionary<string, string> { { "wordQueue" , result } };
                            var frmessage = new Dictionary<string, string> { { "wordQueue", result } };
                            SendMessageToServer(frmessage);
                            wordsToAddToLexicon.Clear();
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
