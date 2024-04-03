using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class DetailsGames
    {
        public GameDto GameDto { get; set; }
        public IEnumerable<GenreDto> Genres { get; set; }

        public IEnumerable<Activity> Activities { get; set; }
    }
}