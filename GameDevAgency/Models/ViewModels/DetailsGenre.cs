﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class DetailsGenre
    {
        // boolean property whether the user is admin or not
        public bool IsAdmin {  get; set; }
        // selected genre with id
        public GenreDto Genre { get; set; }
        // list of games for a genre
        public IEnumerable<GameDto> Games { get; set; }

    }
}