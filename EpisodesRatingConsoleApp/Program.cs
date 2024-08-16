using EpisodesRatingModel;
using System;

namespace EpisodesRatingConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var series = ImdbRetriever.GetById("tt0903747").Result;
        }
    }
}
