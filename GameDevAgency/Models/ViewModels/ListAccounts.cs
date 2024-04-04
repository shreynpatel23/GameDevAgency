using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class ListAccounts
    {
        // boolean property whether the user is admin or not
        public bool IsAdmin { get; set; }
        // the selected user details
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
    }
}