using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PageRank
{
    public class PageRank
    {
        private IDictionary<string, HashSet<string>> pageLinks;
        private int[,] matrix;
        private string[] allPagesLinks;
        private const string BasePath = @"F:\Учеба\stemming\docs";
        private const string BasePathToPageLink = @"F:\Учеба\stemming\docs\response_{0}\element_{0}\links.txt";
        private const string BasePathToOutLink = @"F:\Учеба\stemming\docs\response_{0}\element_{0}\all_links.txt";

        public void CalculatePageRanks()
        {
            Init();
            double PR;
            double sum;
            double eps;
            const double dampingFactor = 0.85; // Коэффициент затухания
            double[] pagesRankOld = new double[this.allPagesLinks.Length];
            double[] pagesRank = new double[this.allPagesLinks.Length];

            

            for (int i = 0; i < this.allPagesLinks.Length; i++)
                pagesRankOld[i] = 1;

            do
            {
                for (int j = 0; j < this.allPagesLinks.Length; j++)
                {
                    sum = 0;
                    pagesRank[j] = (1 - dampingFactor);
                    for (int i = 0; i < this.allPagesLinks.Length; i++)
                    {
                        if (matrix[j, i] > 0)
                            sum += pagesRankOld[i] / SumLink(i);
                        else
                            sum += 0;
                    }
                    sum = dampingFactor * sum;
                    pagesRank[j] += sum;
                }

                eps = pagesRank.Select((t, i) => (t - pagesRankOld[i]) * (t - pagesRankOld[i])).Sum();

                pagesRankOld = pagesRank;
            } while (eps > 0.0001);

            WriteOutput(pagesRank);
        }

        private void WriteOutput(double[] ranks)
        {
            for (int i = 0; i < ranks.Length; i++)
            {
                Console.WriteLine($"{this.allPagesLinks[i]} --- {ranks[i]}");
            }
        }

        private int SumLink(int index)
        {
            var count = 0;
            for (int i = 0; i < allPagesLinks.Length; i++ ) 
                count += matrix[index, i];

            return count;
        }

        private void Init()
        {
            var pagesCount = Directory.GetDirectories(BasePath).Length;
            this.pageLinks = new Dictionary<string, HashSet<string>>();

            var allPages = new List<string>();
            for (int i = 0; i < pagesCount; i++)
            {
                using (var reader = new StreamReader(string.Format(BasePathToPageLink, i)))
                {
                    string line = reader.ReadLine();
                    if (line?.IndexOf("#") > 0)
                        line = line.Substring(0, line.IndexOf("#"));

                    if (!allPages.Contains(line))
                        allPages.Add(line);
                }
            }

            allPagesLinks = new string[allPages.Count];
            allPages.CopyTo(allPagesLinks);
            matrix = new int[allPages.Count, allPages.Count];

            for (int i = 0; i < allPagesLinks.Length; i++)
            {
                using (var reader = new StreamReader(string.Format(BasePathToOutLink, i)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.IndexOf("#") > 0)
                            line = line.Substring(0, line.IndexOf("#"));

                        if (!allPages.Contains(line))
                            continue;

                        // [на кого, кто ссылается]
                        matrix[allPages.IndexOf(line), i] = 1;
                    }
                }
            }
        }
    }
}