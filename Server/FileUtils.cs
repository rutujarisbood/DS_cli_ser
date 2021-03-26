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
            ReadLexicon();
        }

        private static void ReadLexicon()
        {
            ReferenceWords = new Collection<string>(ReadFileContents("C:\\sample files\\serverFile.txt.txt").Split(' '));
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
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-write-to-a-text-file
        public static  void AddWordsToLexicon(string wordsToAdd)
        {
            try
            {
                using StreamWriter file = new StreamWriter("C:\\sample files\\serverFile.txt.txt", append: true);
                file.Write(wordsToAdd);
                //ReadLexicon(); 
            }
            catch (Exception e)
            {
                //
            }
            finally
            {
                ReadLexicon();
            }
        }

        public static void checkForRepeatWords(string wordsToAdd)
        {

        }
    }
}
