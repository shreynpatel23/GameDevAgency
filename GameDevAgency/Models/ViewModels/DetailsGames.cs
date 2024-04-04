using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class DetailsGames
    {
        // boolean property whether the user is admin or not
        public bool IsAdmin { get; set; }
        // selected game with gameId
        public GameDto Game { get; set; }
        // list of genres for a game
        public IEnumerable<GenreDto> Genres { get; set; }
        // list of genres not included in game
        public IEnumerable<GenreDto> AvailableGenres { get; set; }
        // list of activities for a game
        public IEnumerable<ActivityDto> Activities { get; set; }
    }
}