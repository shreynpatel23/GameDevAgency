using GameDevAgency.Models;
using GameDevAgency.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GameDevAgency.Controllers
{
    public class ActivityController : Controller
    {
        // declare http client and js serializer
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private ApplicationDbContext db = new ApplicationDbContext();

        // create a new client and configure the base url
        static ActivityController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44313/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        

        // renders the error page
        // GET  Error
        public ActionResult Error()
        {

            return View();
        }

        // renders the activity list page
        // GET: Activity/List
        public ActionResult List()
        {
            ListActivities ListActivities = new ListActivities();

            // check if the user role is admin or not
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ListActivities.IsAdmin = true;
            else ListActivities.IsAdmin = false;

            // other way to fetch the data is 
            //curl https://localhost:44313/api/ActivityData/GetAllActivities

            // create a url to call api
            string url = "ActivityData/GetAllActivities";

            // get the response 
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create empty activityDTO list and read the data from the response
            IEnumerable<ActivityDto> activities = response.Content.ReadAsAsync<IEnumerable<ActivityDto>>().Result;

            // append the data to the view model
            ListActivities.Activities = activities;

            // send the data in the view
            return View(ListActivities);
        }

        // renders the activity details page
        // GET: Activity/Details/{id}
        public ActionResult Details(int id)
        {
            // declare the view model here
            DetailsActivity DetailsActivity = new DetailsActivity();
            // check if the user role is admin or not
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) DetailsActivity.IsAdmin = true;
            else DetailsActivity.IsAdmin = false;

            // other way to fetch the data is 
            //curl https://localhost:44313/api/ActivityData/GetActivityDetails/2

            // assign the url to a string
            string url = "ActivityData/GetActivityDetails/" + id;

            // use the string to get the response
            HttpResponseMessage response = client.GetAsync(url).Result;


            // create an empty DTO Object and read the data
            ActivityDto activity = response.Content.ReadAsAsync<ActivityDto>().Result;

            // append the data in the view model
            DetailsActivity.Activity = activity;

            // pass the data to the view
            return View(DetailsActivity);
        }

        // renders the add activity page
        // GET Activity/Add
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            // for this page we need to list of games and users in dropdowns
            // so we created a viewModal for AddUpdateActivity.
            // this view modal will have
            // -> selectedActivity (in case of update)
            // -> listOfGames
            // -> ListOfUsers

            // create a new viewmodel for {Games, and Users}
            AddUpdateActivity AddUpdateActivity = new AddUpdateActivity();

            // fetch all the list of users present in the database
            List<ApplicationUser> ApplicationUsers = db.Users.OrderBy(u => u.FirstName).ToList();

            // assign them to the viewmodal
            AddUpdateActivity.Users = ApplicationUsers;

            // fetch all the list of games in the system
            // curl https://localhost:44313/api/GameData/GetAllGames

            string url = "GameData/GetAllGames";

            // get the response
            HttpResponseMessage  response = client.GetAsync(url).Result;

            // create an empty games list and read the data as async
            IEnumerable<Game> Games = response.Content.ReadAsAsync<IEnumerable<Game>>().Result;

            // assign them to the viewmodal
            AddUpdateActivity.Games = Games;

            // pass the view modal to the view
            return View(AddUpdateActivity);
        }


        // use this function to create a new activity in the database
        // GET: Activity/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Activity activity)
        {
            GetApplicationCookie();//get token credentials

            // call the AddActivity api from the data controller
            // curl -H "Content-Type:application/json" -d @activity.json https://localhost:44313/api/ActivityData/AddActivity

            // generate the url string
            string url = "ActivityData/AddActivity";

            // serialize the json payload
            string jsonpayload = jss.Serialize(activity);

            // create a new string content with the serialized json
            HttpContent content = new StringContent(jsonpayload);

            // update the content type to application/json
            content.Headers.ContentType.MediaType = "application/json";

            // get the response from the database
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            // check for status code
            // if success -> go to list page
            // else -> go to the error page
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }


        // renders the edit form
        // GET: Activity/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            // this function again uses the view modal which we created for add activity
            // the only difference is it will also have the selected activity to populate the form

            // create a new viewmodel for {Games, and User}
            AddUpdateActivity AddUpdateActivity = new AddUpdateActivity();

            // fetch the activity details from the given id
            //curl https://localhost:44313/api/ActivityData/GetActivityDetails/{id}
            string url = "ActivityData/GetActivityDetails/" + id;

            // get the result
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create an empty ActivityDTO and store the data in it
            ActivityDto SelectedActivity = response.Content.ReadAsAsync<ActivityDto>().Result;

            // convert the dueDate 
            ViewData["DueDate"] = Convert.ToDateTime(SelectedActivity.ActivityDueDate).ToString("yyyy-MM-dd");

            // assign the selectedActivity to the view modal
            AddUpdateActivity.SelectedActivity = SelectedActivity;

            // fetch all the list of users present in the database
            List<ApplicationUser> ApplicationUsers = db.Users.OrderBy(u => u.FirstName).ToList();

            // assign them to the viewmodal
            AddUpdateActivity.Users = ApplicationUsers;

            // fetch all the list of games in the system
            // curl https://localhost:44313/api/GameData/GetAllGames

            url = "GameData/GetAllGames";

            // get the response
            response = client.GetAsync(url).Result;

            // create an empty project list and read the data as async
            IEnumerable<Game> Games = response.Content.ReadAsAsync<IEnumerable<Game>>().Result;

            // assign them to the viewmodal
            AddUpdateActivity.Games = Games;

            // send the view modal to the view
            return View(AddUpdateActivity);
        }

        // us this function to update the record in the database
        // POST: Activity/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Activity activity)
        {
            try
            {
                GetApplicationCookie();//get token credentials

                // call the update activity api using the following api
                // curl -H "Content-Type:application/json" -d @activity.json https://localhost:44313/api/ActivityData/UpdateActivity/2
                string url = "ActivityData/UpdateActivity/" + id;

                // serialize the json payload
                string jsonpayload = jss.Serialize(activity);

                // assign serialized payload to string
                HttpContent content = new StringContent(jsonpayload);

                // update the content type to application/json
                content.Headers.ContentType.MediaType = "application/json";

                // get the result and assign the response.
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                // once done redirect to the details page
                return RedirectToAction("Details/" + id);
            }
            catch
            {
                return View();
            }
        }

        // render the confirm activity delete page
        // GET: Activity/ConfirmDelete/5
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmDelete(int id)
        {
            // call the get activity details api to fill the data on the confirm delete page
            string url = "ActivityData/GetActivityDetails/" + id;

            // get the result
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create an activity dto object and read the data
            ActivityDto activity = response.Content.ReadAsAsync<ActivityDto>().Result;

            // send the data to view
            return View(activity);
        }

        // delete an activity from the database
        // POST: Activity/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, Activity activity)
        {
            try
            {
                GetApplicationCookie();//get token credentials

                // call the delete activity api
                // curl -d "" https://localhost:44313/api/ActivityData/DeleteActivity/2
                string url = "ActivityData/DeleteActivity/" + id;

                // as this is a delete request the string content will be an empty string
                HttpContent content = new StringContent("");

                // change the content type to application json
                content.Headers.ContentType.MediaType = "application/json";

                // read the data 
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                // check the status code
                // if sucess -> redirect to list
                // else -> redirect to error
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List");
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
