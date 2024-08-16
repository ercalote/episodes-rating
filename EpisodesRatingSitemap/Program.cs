using EpisodesRatingModel;
using System;
using System.IO;
using System.Linq;

namespace EpisodesRatingSitemap
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dbContext = new EpisodesRatingContext())
            {
                File.WriteAllLines(
                    "sitemap.txt",
                    dbContext.Series
                        .Select(x => $"https://episodesrating.com/{x.ImdbId}/{x.Friendly}")
                        .ToArray());
            }
        }
    }
}
