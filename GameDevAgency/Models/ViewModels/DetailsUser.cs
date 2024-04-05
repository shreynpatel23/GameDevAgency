using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class DetailsUser
    {
        // boolean property whether the user is admin or not
        public bool IsAdmin { get; set; }
        // the selected user details
        public ApplicationUser ApplicationUser { get; set; }
        // roles of the user
        public IEnumerable<string> Roles { get; set; }
        // activities of a particular user
        public IEnumerable<ActivityDto> Activities {  get; set; }

    }
}