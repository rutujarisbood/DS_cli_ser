// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace MyServer
{
    static class FileUtils
    {
        // using Map/Dictionary to avoid checking duplicates manually.
        private static Dictionary<string, string> ReferenceWords;
        static FileUtils()
        {
            ReadLexicon();
        }

        private static void ReadLexicon()
        {
            var words = new Collection<string>(ReadFileContents("C:\\sample files\\serverFile.txt.txt").Split(' '));

            ReferenceWords = words.ToDictionary<string, string>(value => value);
        }

        public static string ReadFileContents(string filename)
        {
            return File.ReadAllText(filename);
        }

        public static string PerformSpellCheck(string filecontents)
        {

            if (!string.IsNullOrEmpty(filecontents))
            {
                var words = filecontents.Split(' ');
                for (var i = 0; i < words.Length; i++)
                {
                    if (ReferenceWords.ContainsKey(words[i]))
                    {
                        words[i] = "[" + words[i] + "]";
                    }
                }

                return String.Join(" ", words);
            }
            return filecontents;
        }

        public static void AddToLexicon(string[] words)
        {
            // Since we switched the ReferenceWords to a dictionary, we don't need to worry about the duplicates. They are overwritten.
            if (words != null && words.Length > 0)
            {
                // lamda type for loop
                words.ToList().ForEach(x => ReferenceWords[x] = x);
                try
                {
                    File.WriteAllText("C:\\sample files\\serverFile.txt.txt", String.Join(' ', ReferenceWords.Keys));
                }
                catch (Exception e)
                {
                    Console.WriteLine("error in writing file");
                }
                finally
                {
                    ReadLexicon();
                }
            }
        }

    }
}
