// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MyServer
{
    public class ClientHandler
    {
        private Socket client;
        private static Socket sock;
        private static List<string> listOfUserName = new List<string>();
        private string username;

        public void pollClient()
        {
            while (this.client.Connected)
            {

                Dictionary<string, string> responseToSend = new Dictionary<string, string>();
                responseToSend.Add("poll", "checkQueue");
                this.client.SendMessage(responseToSend);

                Thread.Sleep(6000);

            }
        }
        public ClientHandler(Socket client)
        {
            this.client = client;

        }


        public void Run()
        {
            try
            {
                while (true)
                {
                    var message = this.client.ReadMessage();
                    if (message != null)
                    {
                        //trial
                        //SendMessageToBkp(message);
                        if (message.ContainsKey("exit"))
                        {
                            username = message["exit"];
                            Server.log("User disconnected: " + username);
                            listOfUserName.Remove(username);
                            if (listOfUserName.Count > 0)
                            {
                                Server.log("list of currently connected users : ");
                                foreach (string user in listOfUserName)
                                {
                                    Server.log(user + "\n");
                                }
                            }
                            else
                            {
                                Server.log("no active user connection available");
                            }
                        }
                        else if (message.ContainsKey("wordQueue"))
                        {
                            // getting string, splitting and passing to function.
                            var wordsToAdd = message["wordQueue"];
                            Server.log("adding words to lexicon : " + wordsToAdd);
                            var words = wordsToAdd.Split(" ");
                            FileUtils.AddToLexicon(words);
                            Server.messagesForBackup.Add(wordsToAdd);
                        }
                        else if (message.ContainsKey("updateData"))
                        {
                            // This code is for backup server only
                            var wordsToAdd = message["words"];
                            FileUtils.AddToLexicon(wordsToAdd.Split(" "));
                        }
                        else
                        {
                            username = message.ContainsKey("register") ? message["register"] : null;
                            if (string.IsNullOrEmpty(username))
                            {
                                if (message.ContainsKey("file"))
                                {
                                    Server.log($"Received file from user {message["username"]}:{Environment.NewLine}{message["file"]}");
                                    var resp = ProcessMessage(message);

                                    this.client.SendMessage(resp);
                                }
                            }
                            else
                            {
                                // this if will only run for backup.
                                if (message.ContainsKey("isFromMainServer") && message["isFromMainServer"] == "Yes")
                                {

                                }
                                else
                                {

                                    if (IsUserNameUnique(username))
                                    {
                                        listOfUserName.Add(username);
                                        Dictionary<string, string> responseToSend = new Dictionary<string, string>();

                                        responseToSend.Add("status", "Accepted");
                                        Server.log("User connected: " + username);
                                        Console.WriteLine("User connected: " + username);
                                        //responseToSend.Add("BackupPort", ConfigurationManager.AppSettings.Get("BackupPort"));
                                        this.client.SendMessage(responseToSend);
                                        //ref https://www.csharp-examples.net/create-new-thread/
                                        Thread thread = new Thread(new ThreadStart(pollClient));
                                        thread.Start();

                                    }
                                    else
                                    {
                                        Dictionary<string, string> responseToSend = new Dictionary<string, string>();
                                        responseToSend.Add("status", "Rejected");
                                        Server.log("User rejected:");
                                        this.client.SendMessage(responseToSend);

                                    }
                                }

                            }
                        }

                    }

                }
            }
            catch (Exception e)
            {
                if (Server.isPrimary)
                {
                    Server.log(e.ToString());
                    Server.log($"Client {username} disconnected");
                }
            }

        }
        private Boolean IsUserNameUnique(string username)
        {
            if (listOfUserName.Contains(username))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private Dictionary<string, string> ProcessMessage(Dictionary<string, string> message)
        {
            //logic to check repeated user names
            var fileContents = message.ContainsKey("file") ? message["file"] : null;
            var respFileContents = FileUtils.PerformSpellCheck(fileContents);
            Dictionary<string, string> responseToSend = new Dictionary<string, string>();
            if (respFileContents != null)
            {
                responseToSend.Add("status", "Done");
                responseToSend.Add("file", respFileContents);
            }
            else
            {
                responseToSend.Add("status", "Failed");
                responseToSend.Add("file", "");
            }

            return responseToSend;
        }
    }
}