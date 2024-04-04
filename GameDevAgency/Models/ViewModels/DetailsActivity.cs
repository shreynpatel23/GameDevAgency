using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class DetailsActivity
    {
        // boolean property whether the user is admin or not
        public bool IsAdmin { get; set; }
        // activity details with an id
        public ActivityDto Activity { get; set; }
    }
}