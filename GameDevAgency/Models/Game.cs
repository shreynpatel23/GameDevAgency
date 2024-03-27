using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models
{
    public class Game
    {
        // unique identifier for a game
        [Key]
        public int GameId { get; set; }

        // the version of the game eg. v1
        public string GameVersion { get; set; }

        // the name of the game
        public string GameName { get; set; }

        // the description of the game
        public string GameDescription { get; set; }
        // the date on which the game was released
        public DateTime GameReleaseDate { get; set; }

        // list of genres for a game
        // A game can have many Genres <-> A Genre can have many Games
        public ICollection<Genre> Genres { get; set; }
    }

    public class GameDto
    {
        // id of the game
        public int GameId { get; set; }

        // the version of the game eg. v1
        public string GameVersion { get; set; }

        // the name of the game
        public string GameName { get; set; }

        // the description of the game
        public string GameDescription { get; set; }
        // the date on which the game was released
        public DateTime GameReleaseDate { get; set; }

    }
}