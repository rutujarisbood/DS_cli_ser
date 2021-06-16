// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace MyServer
{

    public static class Utils
    {
        public static void SendMessage(this Socket client, Dictionary<string, string> message)
        {
            using (StreamWriter writer = new StreamWriter(new NetworkStream(client)))
            {
                writer.WriteLine(JsonConvert.SerializeObject(message));
                writer.Flush();
            }
        }

        public static Dictionary<string, string> ReadMessage(this Socket client)
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
    }
}
