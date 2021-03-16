// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace MyServer
{
    static class FileUtils
    {
        private static Collection<string> ReferenceWords;
        static FileUtils()
        {
            ReferenceWords =  new Collection<string>(ReadFileContents("C:\\sample files\\serverFile.txt.txt").Split(' '));
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
                for (var i = 0; i< words.Length; i++ )
                {
                    if (ReferenceWords.Contains(words[i]))
                    {
                        words[i] = "[" + words[i] + "]";
                    }
                }

                return String.Join(" ", words);
            }
            return filecontents;
        }
    }
}
