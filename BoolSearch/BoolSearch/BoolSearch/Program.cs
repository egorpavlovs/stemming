using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Checksums;

namespace BoolSearch
{
    class Program
    {
        private const string PathToDocs = "F:\\Учеба\\stemming\\stemming_docs";
        private const string PathToTable = "F:\\Учеба\\stemming\\BoolSearch\\BoolSearch";

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to documents.");
            string pathToFiles = Console.ReadLine();
            if (!Directory.Exists(pathToFiles))
            {
                pathToFiles = Program.PathToDocs;
            }

            Console.WriteLine("Enter the path to store table.");
            var pathToTable = Console.ReadLine();
            if (!Directory.Exists(pathToTable))
            {
                pathToTable = Program.PathToTable;
            }

            var files = Directory.GetFiles(pathToFiles, "*", SearchOption.AllDirectories);

            Console.WriteLine("Preparing table.");
            TableGenerator tg = new TableGenerator(pathToTable);
            tg.PrepareTable(files);

            Console.WriteLine("Write your query");
            var query = Console.ReadLine();

            var res = tg.Search(query);

            GetLinks(res.ToArray()).ForEach(Console.WriteLine);
            Console.ReadLine();
        }

        private static List<string> GetLinks(string[] paths)
        {
            var set = new HashSet<string>();
            foreach (var path in paths)
            {
                var newPath = path.Replace("content", "links");
                using (StreamReader reader = new StreamReader(newPath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        set.Add(line);
                    }
                }
            }

            return set.ToList();
        }
    }
}
