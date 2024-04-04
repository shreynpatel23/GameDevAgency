using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class ListActivities
    {
        // flag to check whether the user is admin
        public bool IsAdmin { get; set; }
        // List of genres
        public IEnumerable<ActivityDto> Activities { get; set; }

    }
}