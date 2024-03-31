using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class AddUpdateActivity
    {
        // selected activity details
        public ActivityDto SelectedActivity { get; set; }
        // list of all collaborators in the system
        public IEnumerable<ApplicationUser> Users { get; set; }
        // list of all projects in the system
        public IEnumerable<Game> Games { get; set; }
    }
}