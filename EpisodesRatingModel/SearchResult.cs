using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpisodesRatingModel
{
    public class SearchResult
    {
        public List<Search> Search { get; set; }
    }

    public class Search
    {
        public string Title { get; set; }

        [JsonProperty("imdbID")]
        public string ImdbId { get; set; }

        [JsonProperty("Poster")]
        public string ImageUrl { get; set; }

        public string Year { get; set; }

        public string Friendly =>
            new string(this.Title.ToLowerInvariant().Replace(' ', '-').Where(x => x == '-' || (x >= 'a' && x <= 'z')).ToArray());
    }
}
