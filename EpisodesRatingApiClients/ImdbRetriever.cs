using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EpisodesRatingModel
{
    public static class ImdbRetriever
    {
        private static HttpClient Client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });

        public static async Task<Series> GetById(string imdbId)
        {
            var htmls = new List<string>();

            var series = new Series
            {
                ImdbId = imdbId,
                LastUpdated = DateTime.UtcNow
            };

            var json = await (await Client.GetAsync($"http://www.omdbapi.com/?i={imdbId}&apikey=45f39e00")).Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(json);

            if (string.IsNullOrWhiteSpace(data.Title))
            {
                throw new Exception($"No title found for '{imdbId}': {data}.");
            }

            series.Title = data.Title;
            series.Description = data.Plot;
            series.ImageUrl = data.Poster;

            try
            {
                series.Rating = double.Parse(data.imdbRating, CultureInfo.InvariantCulture);
            }
            catch
            {
                series.Rating = null;
            }

            series.VotesCount = data.imdbVotes;
            series.Year = data.Year;

            var season = 1;
            var nrSeasons = 0;
            do
            {
                var url = $"https://www.imdb.com/title/{imdbId}/episodes/?season={season}";
                var response = await Client.GetAsync(url);
                if ((int)response.StatusCode == 301 || (int)response.StatusCode == 302 || (int)response.StatusCode == 307 || (int)response.StatusCode == 308)
                {
                    url = $"https://www.imdb.com/{response.Headers.Location}?season={season}";
                    response = await Client.GetAsync(url);
                }
                var html = await response.Content.ReadAsStringAsync();
                htmls.Add(html);
                season++;

                if (nrSeasons == 0)
                {
                    nrSeasons = Regex.Matches(html, "data-testid=\"tab-season-entry\"").Count;
                }
            } while (season <= nrSeasons);

            if (!htmls.Any())
            {
                throw new Exception($"No seasons information for '{imdbId}'.");
            }

            var articles = htmls
                .Select(x =>
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(x);
                    return htmlDoc;
                })
                .SelectMany(q => q
                    .DocumentNode
                    .Descendants("article"))
                .ToList();

            var episodesRaw = articles
                    .Select(x => new
                    {
                        Title = x
                            .Descendants("h4")
                            .SingleOrDefault()
                            ?.InnerText
                            .Split('∙')
                            .Last()
                            .Trim(),
                        Description = x
                            .Descendants("div")
                            .SingleOrDefault(y => y.HasClass("ipc-html-content-inner-div"))
                            ?.InnerText
                            .Trim(),
                        SeasonEpisode = x
                            .Descendants("a")
                            .SingleOrDefault(y => y.InnerText.StartsWith("S") && y.InnerText.Contains(".E"))
                            ?.InnerText
                            .Split(' ')
                            .First()
                            .Split('.'),
                        RatingDiv = x
                            .Descendants("span")
                            .SingleOrDefault(y => y.HasAttributes && (y.Attributes["aria-label"]?.Value.StartsWith("IMDb rating:") ?? false)),
                        ImdbLink = x
                            .Descendants("a")
                            .FirstOrDefault()
                            ?.Attributes["href"]
                            ?.Value,
                        ImageUrl = x
                            .Descendants("img")
                            .FirstOrDefault()
                            ?.Attributes["src"]
                            ?.Value,
                        AirDate = x
                            .Descendants("h4")
                            .FirstOrDefault()
                            ?.NextSibling
                            ?.InnerText
                            .Trim()
                    })
                    .ToList();

            var episodes = episodesRaw
                    .Where(x => x.Title != null)
                    .Select(x => new
                    {
                        Season = int.Parse(new string(x.SeasonEpisode[0].Where(char.IsDigit).ToArray())),
                        Episode = int.Parse(new string(x.SeasonEpisode[1].Where(char.IsDigit).ToArray())),
                        Rating = x.RatingDiv == null ? (double?)null : double.Parse(
                            x.RatingDiv.Attributes["aria-label"].Value.Split(' ').Last(),
                            CultureInfo.InvariantCulture),
                        Votes = x.RatingDiv == null ? (int?)null : int.Parse(new string(
                            x.RatingDiv.Descendants("span").Last().InnerText.Replace("K", "000").Where(char.IsDigit).ToArray())),
                        x.Title,
                        x.Description,
                        x.ImdbLink,
                        x.ImageUrl,
                    })
                    .OrderBy(x => x.Season)
                    .ThenBy(x => x.Episode)
                    .ToList();

            series.Episodes = episodes
                .Select(x => new Episode
                {
                    Title = x.Title,
                    Description = x.Description,
                    SeasonNumber = x.Season,
                    EpisodeNumber = x.Episode,
                    Rating = x.Rating,
                    VotesCount = x.Votes,
                    ImdbLink = $"https://www.imdb.com{x.ImdbLink}",
                    ImageUrl = x.ImageUrl,
                    Series = series
                })
                .ToList();

            return series;
        }
    }
}
