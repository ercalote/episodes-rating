using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EpisodesRatingModel
{
    public class Episode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int EpisodeId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("season")]
        public int SeasonNumber { get; set; }

        [JsonProperty("episode")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("rating")]
        public double? Rating { get; set; }

        [JsonProperty("votes")]
        public int? VotesCount { get; set; }

        [JsonProperty("image")]
        public string ImageUrl { get; set; }

        [JsonProperty("imdb_link")]
        public string ImdbLink { get; set; }

        [JsonProperty("airdate")]
        public string AirDate { get; set; }

        [JsonIgnore]
        public Guid SeriesId { get; set; }

        [JsonIgnore]
        public virtual Series Series { get; set; }
    }
}