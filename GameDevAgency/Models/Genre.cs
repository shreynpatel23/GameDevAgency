using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models
{
    public class Genre
    {

        // unique identifier for Genre table
        [Key]
        public int GenreId { get; set; }

        // the name of the Genre
        public string GenreName { get; set; }

        // list of games for a genre
        // A Genre can have many Games <-> A game can have many Genres
        public ICollection<Game> Games { get; set; }
    }

    public class GenreDto
    {
        // id of the Genre
        public int GenreId { get; set; }

        // the name of the Genre
        public string GenreName { get; set; }
    }
}