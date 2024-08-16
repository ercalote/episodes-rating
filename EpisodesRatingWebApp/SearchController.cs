using EpisodesRatingModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EpisodesRatingWebApp
{
    public class SearchController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetByImdbId(string imdbId)
        {
            var httpClient = new HttpClient();

            ViewBag.LambdaResult = await httpClient.GetAsync(new Uri($"http://localhost:7071/api/GetFromImdb?imdbId={imdbId}"));

            return new ViewResult();
        }
    }
}
