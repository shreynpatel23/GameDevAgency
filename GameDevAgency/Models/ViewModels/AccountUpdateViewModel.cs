using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models.ViewModels
{
    public class AccountUpdateViewModel
    {
        // application user details
        // user id of the user
        public string UserId {  get; set; }
        // first name of the user
        public string FirstName{ get; set; }
        // last name of the user
        public string LastName { get; set; }
        // email of the user
        public string Email { get; set; }
        // phone number of the user
        public string PhoneNumber { get; set; } 
        // user name of the user
        public string UserName {  get; set; }
        // the current role assigned to the user
        public string UserRole {  get; set; }
        // list of roles
        public List<IdentityRole> Roles { get; set; }
    }
}