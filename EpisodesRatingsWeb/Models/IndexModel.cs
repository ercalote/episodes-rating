using EpisodesRatingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EpisodesRatingsWeb.Models
{
    public class IndexModel
    {
        public Series Series { get; set; }

        public List<Series> OtherSeries { get; set; }
    }
}
