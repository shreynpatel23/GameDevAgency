using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class DetailsGenre
    {
        public GenreDto Genre { get; set; }
        public IEnumerable<GameDto> Games { get; set; }

    }
}