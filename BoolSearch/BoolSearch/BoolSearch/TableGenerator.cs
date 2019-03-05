using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BoolSearch
{
    public class TableGenerator
    {
        private static char[] separators = { ' ', ',', '.', ';', '^', '`', ':', '?', '&', '!', '+', '-', '_', '#', '<', '>', '/', '|', '\\', '"', '(', ')', '[', ']', '=', '*', '%', '\t' };
        private readonly string pathToTable;
        private const string TableFileName = "table.txt";

        public TableGenerator(string pathToTable)
        {
            this.pathToTable = pathToTable;
        }

        public List<string> Search(string queryString)
        {
            var files = new List<string>();
            var wordsToSearch = queryString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            List<int> wordsPositions = new List<int>();
            // Reading header.
            using (var reader = new StreamReader(Path.Combine(pathToTable, TableFileName)))
            {
                var line = reader.ReadLine();
                var words = line?.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (wordsToSearch.Contains(words[i]))
                        wordsPositions.Add(i);
                }

                while ((line = reader.ReadLine()) != null)
                {
                    var existense = line.Split(' ');
                    if (wordsPositions.TrueForAll(p =>
                        existense[p + 1].Equals("1", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        files.Add(existense[0]);
                    }
                }
            }

            return files;
        }

        public void PrepareTable(string[] files)
        {
            var words = GenerateDictionary(files);
            using (var writer = new StreamWriter(Path.Combine(pathToTable, TableFileName)))
            {
                // Adding header
                writer.WriteLine(string.Join(" ", words));
                foreach (var file in files)
                {
                    var wordExists = new int[words.Count];
                    string text;
                    using (var reader = new StreamReader(file))
                    {
                        text = reader.ReadToEnd();
                    }

                    for (int i = 0; i < wordExists.Length; i++)
                    {
                        wordExists[i] = text.Contains(words[i]) ? 1 : 0;
                    }

                    var result = string.Join(" ", wordExists);
                    writer.WriteLine($"{file} {result}");
                }
            }


        }

        private List<string> GenerateDictionary(string[] files)
        {
            var words = new HashSet<string>();
            foreach (var f in files)
            {
                using (var reader = new StreamReader(f))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        foreach (var s in line.Split(separators))
                        {
                            words.Add(s);
                        }
                    }
                }
            }

            return words.ToList();
        }
    }
}
