using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EpisodesRatingModel
{
    public class Series
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public Guid SeriesId { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonIgnore]
        public DateTime LastUpdated { get; set; }

        [JsonIgnore]
        public int Views { get; set; }

        [JsonProperty("rating")]
        public double? Rating { get; set; }

        [JsonProperty("votes")]
        public string VotesCount { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("episodes")]
        public virtual List<Episode> Episodes { get; set; }

        public double? AvgEpisodeRating =>
            this.Episodes.Any(x => x.Rating.HasValue)
                ? this.Episodes.Where(x => x.Rating.HasValue).Average(x => x.Rating.Value)
                : (double?)null;

        public string Friendly =>
            new string(this.Title.ToLowerInvariant().Replace(' ', '-').Where(x => x == '-' || (x >= 'a' && x <= 'z')).ToArray());


        public string ToJson()
        {
            return JsonConvert.SerializeObject(
                this,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
    }
}
