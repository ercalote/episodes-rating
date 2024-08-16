using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EpisodesRatingsWeb.Models;
using System.Net.Http;
using Newtonsoft.Json;
using EpisodesRatingModel;
using Microsoft.EntityFrameworkCore;
using System.Data.Linq.Mapping;
using System.Text;

namespace EpisodesRatingsWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        private List<Series> GetRandomSeries()
        {
            using (var dbContext = new EpisodesRatingContext())
            {
                return dbContext.Series
                    .Include(x => x.Episodes)
                    .OrderBy(x => Guid.NewGuid())
                    .Where(x => x.Episodes.Count > 50 && x.Views > 10)
                    .Take(10)
                    .ToList();
            }
        }

        public IActionResult Index()
        {
            var randomSeries = this.GetRandomSeries();
            var series = randomSeries.First();
            series.Episodes = series.Episodes.Where(x => x.EpisodeNumber > 0).ToList();

            return View(
                new IndexModel
                {
                    Series = series,
                    OtherSeries = randomSeries.Except(new[] { series }).ToList()
                });
        }

        [HttpGet]
        public async Task<IActionResult> GetByImdbId(string id, string friendly)
        {
            if (id == "robots.txt" && friendly == null)
            {
                var builder = new StringBuilder();

                builder.AppendLine("User-agent: *");
                builder.AppendLine("Disallow: ");
                builder.AppendLine("Sitemap: https://episodesrating.com/sitemap.txt");

                return this.Content(builder.ToString(), "text/plain", Encoding.UTF8);
            }
            else if (id == "sitemap.txt" && friendly == null)
            {
                using (var dbContext = new EpisodesRatingContext())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"https://episodesrating.com/");

                    foreach (var x in dbContext.Series)
                    {
                        builder.AppendLine($"https://episodesrating.com/{x.ImdbId}/{x.Friendly}");
                    }

                    return this.Content(builder.ToString(), "text/plain", Encoding.UTF8);
                }
            }

            //var functionUrl = $"http://localhost:7071/api/GetFromImdb?imdbId={id}";
            var functionUrl = $"https://episodesratingfunctionapp20200413134134.azurewebsites.net/api/GetFromImdb?code=DYaMprTPUbaUzXTUxcy2wLUCyAu35ijAmcfH7hwUcauRqu94/kIMsg==&imdbId={id}";

            var response = await Program.HttpClient.GetAsync(new Uri(functionUrl));

            var json = await response.Content.ReadAsStringAsync();
            var series = JsonConvert.DeserializeObject<Series>(json);
            series.Episodes = series.Episodes.Where(x => x.EpisodeNumber > 0).ToList();

            return View(
                "Index",
                new IndexModel
                {
                    Series = series,
                    OtherSeries = this.GetRandomSeries()
                });
        }

        [HttpPost]
        [Route("/Home/Search")]
        public async Task<IActionResult> Search(string title)
        {
            title = title.Trim();
            var response = await Program.HttpClient.GetAsync(new Uri($"http://www.omdbapi.com/?s=*{title}*&type=series&apikey=45f39e00"));
            var json = await response.Content.ReadAsStringAsync();
            var searchResult = JsonConvert.DeserializeObject<SearchResult>(json);

            var result = searchResult.Search.Select(x =>
                new
                {
                    value = x.Title,
                    label = x.ImageUrl,
                    id = x.ImdbId,
                    friendly = x.Friendly,
                    year = x.Year
                });

            return Json(result);
        }

        public IActionResult Error()
        {
            return View(
                new IndexModel
                {
                    OtherSeries = this.GetRandomSeries()
                });
        }
    }
}
