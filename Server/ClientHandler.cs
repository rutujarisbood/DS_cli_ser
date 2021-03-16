// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MyServer
{
    public class ClientHandler
    {
        private Socket client;
        private static List<string> listOfUserName = new List<string>();
        private string username;
        public ClientHandler(Socket client)
        {
            this.client = client;
        }

        private void SendMessage(Dictionary<string, string> message)
        {
            using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
            {
                writer.WriteLine(JsonConvert.SerializeObject(message));
                writer.Flush();
            }
        }

        private Dictionary<string, string> ReadMessage()
        {
            try
            {
                // https://stackoverflow.com/questions/1450263/c-sharp-streamreader-readline-does-not-work-properly

                using (StreamReader reader = new StreamReader(new NetworkStream(client)))
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

        public void Run()
        {
            try
            {
                while (true)
                {
                    var message = ReadMessage();
                    if (message != null)
                    {
                        if (message.ContainsKey("exit"))
                        {
                            username = message["exit"];
                            Server.log("User disconnected: " + username);
                            listOfUserName.Remove(username);
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
                                    SendMessage(resp);
                                }
                            }
                            else
                            {

                                if (IsUserNameUnique(username))
                                {
                                    listOfUserName.Add(username);
                                    Dictionary<string, string> responseToSend = new Dictionary<string, string>();

                                    responseToSend.Add("status", "Accepted");
                                    Server.log("User connected: " + username);
                                    //responseToSend.Add("file", null);
                                    SendMessage(responseToSend);
                                }
                                else
                                {
                                    Dictionary<string, string> responseToSend = new Dictionary<string, string>();
                                    responseToSend.Add("status", "Rejected");
                                    Server.log("User rejected:");
                                    //responseToSend.Add("file", null);
                                    SendMessage(responseToSend);

                                }

                            }
                        }
                        
                    }

                }
            }
            catch (Exception)
            {
                Server.log($"Client {username} disconnected");
                throw;
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
            //SendMessage(responseToSend);//9096
            return responseToSend;

        }
    }
}