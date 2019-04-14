using System.Linq;

namespace PageRank
{
    class PageRanker
    {
        private double PageRank(int[,] a, int n, int[] sumLink)
        {
            double PR;
            double sum;
            double eps;
            const double dampingFactor = 0.85; // Коэффициент затухания
            double[] pagesRankOld = new double[n];
            double[] pagesRank = new double[n];

            for (int i = 0; i < n; i++)
                pagesRankOld[i] = 1;

            do
            {
                for (int j = 0; j < n; j++)
                {
                    sum = 0;
                    pagesRank[j] = (1 - dampingFactor);
                    for (int i = 0; i < n; i++)
                    {
                        if (a[j, i] > 0)
                            sum += pagesRankOld[i] / sumLink[i];
                        else
                            sum += 0;
                    }
                    sum = dampingFactor * sum;
                    pagesRank[j] += sum;
                }

                PR = pagesRank.Sum();
                eps = pagesRank.Select((t, i) => (t - pagesRankOld[i]) * (t - pagesRankOld[i])).Sum();

                pagesRankOld = pagesRank;
            } while (eps > 0.0001);

            return PR;
        }
    }
}
