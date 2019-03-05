using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BoolSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to documents.");
            string pathToFiles = Console.ReadLine();
            if (!Directory.Exists(pathToFiles))
            {
                throw new DirectoryNotFoundException();
            }

            Console.WriteLine("Enter the path to store table.");
            var pathToTable = Console.ReadLine();
            if (!Directory.Exists(pathToTable))
            {
                throw new DirectoryNotFoundException();
            }

            var files = Directory.GetFiles(pathToFiles, "*", SearchOption.AllDirectories);

            Console.WriteLine("Preparing table.");
            TableGenerator tg = new TableGenerator(pathToTable);
            tg.PrepareTable(files);

            Console.WriteLine("Write your query");
            var query = Console.ReadLine();

            var res = tg.Search(query);

            res.ForEach(Console.WriteLine);
            Console.ReadLine();
        }
    }
}
