using System;

namespace PageRank
{
    class Program
    {
        static void Main(string[] args)
        {
            PageRank pr = new PageRank();
            pr.CalculatePageRanks();
            Console.ReadLine();
        }
    }
}
