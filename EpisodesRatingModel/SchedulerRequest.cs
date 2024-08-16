using System;
using System.Collections.Generic;
using System.Text;

namespace EpisodesRatingModel
{
    public class SchedulerRequest
    {
        public Guid SeriesId { get; set; }

        public string ImdbId { get; set; }

        public string Title { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Views { get; set; }

        public static SchedulerRequest FromSeries(Series series)
        {
            return new SchedulerRequest
            {
               ImdbId = series.ImdbId,
               LastUpdated = series.LastUpdated,
               SeriesId = series.SeriesId,
               Title = series.Title,
               Views = series.Views
            };
        }
    }
}
