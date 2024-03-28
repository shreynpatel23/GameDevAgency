using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameDevAgency.Models
{
    public class Activity
    {
        [Key]
        // The primary key of activity
        public int ActivityId { get; set; }

        // The name of the activity
        public string ActivityName { get; set; }

        // A short description to explan the activity
        public string ActivityDescription { get; set; }

        // The priority of the Activity (Easy, Medium, Hard).
        public string ActivityPriority { get; set; }

        // Current Status of the Activity (Todo, In-progress, Done).
        public string ActivityStatus { get; set; }

        // Due Date of the activity
        public DateTime ActivityDueDate { get; set; }

        // Estimates to how long it will take to complete the activity
        // Estimates are in HOURS
        public string ActivityEstimates { get; set; }

        // An activity belongs to one game
        // A Game can have many activities
        [ForeignKey("Game")]
        public int GameId { get; set; }
        public virtual Game Game { get; set; }

        // An Activity belongs to one user
        // A user can have many activities
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class ActivityDto
    {
        // The primary key of activity
        public int ActivityId { get; set; }

        // The name of the activity
        public string ActivityName { get; set; }

        // A short description to explan the activity
        public string ActivityDescription { get; set; }

        // The priority of the Activity (Easy, Medium, Hard).
        public string ActivityPriority { get; set; }

        // Current Status of the Activity (Todo, In-progress, Done).
        public string ActivityStatus { get; set; }

        // Due Date of the activity
        public DateTime ActivityDueDate { get; set; }

        // Estimates to how long it will take to complete the activity
        // Estimates are in HOURS
        public string ActivityEstimates { get; set; }

        // user id
        public int UserId { get; set; }

        // first name of the user
        public string FirstName { get; set; }

        // last name of the user
        public string LastName { get; set; }

        // role of the user
        public string Role { get; set; }

        // Game id
        public int GameId { get; set; }

        // Name of the Game in which the task is
        public string GameName { get; set; }

        // release date of the game
        public DateTime GameReleaseDate { get; set; }
    }
}