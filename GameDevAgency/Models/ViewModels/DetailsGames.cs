using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class DetailsGames
    {
        public bool IsAdmin { get; set; }
        public GameDto Game { get; set; }
        public IEnumerable<GenreDto> Genres { get; set; }
        public IEnumerable<GenreDto> AvailableGenres { get; set; }

        public IEnumerable<Activity> Activities { get; set; }
    }
}