using GameDevAgency.Models;
using GameDevAgency.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GameDevAgency.Controllers
{
    public class GameController : Controller
    {
        // declare http client and js serializer
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        // create a new client and configure the base url
        static GameController()
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


        // error page
        // GET  Error
        public ActionResult Error()
        {

            return View();
        }

        // GET: Game/List
        public ActionResult List()
        {
            // declare the view model
            ListGames ListGames = new ListGames();

            // check if the user role is admin or not
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ListGames.IsAdmin = true;
            else ListGames.IsAdmin = false;


            // make a curl request to
            // https://localhost:44313/api/GameData/GetAllGames

            // create a url to call api
            string url = "GameData/GetAllGames";

            // get the response 
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create empty GameDta list and read the data from the response
            IEnumerable<GameDto> games = response.Content.ReadAsAsync<IEnumerable<GameDto>>().Result;

            // append the games list to the view model
            ListGames.Games = games;

            // send the data in the view
            return View(ListGames);
        }

        // GET: Game/Details/5
        public ActionResult Details(int id)
        {
            // declare the view model here
            DetailsGames DetailsGames = new DetailsGames();

            // check if the user role is admin or not
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) DetailsGames.IsAdmin = true;
            else DetailsGames.IsAdmin = false;

            // make a curl request to
            // https://localhost:44313/api/GameData/GetGameDetails/4

            // assign the url to a string
            string url = "GameData/GetGameDetails/" + id;

            // use the string to get the response
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create an empty DTO Object and read the data
            GameDto game = response.Content.ReadAsAsync<GameDto>().Result;

            // assing it to the view model
            DetailsGames.Game = game;


            // FETCH LIST OF GENRES
            // fetch the list of genres present for a game id
            url = "GenreData/GetAllGenresForGame/" + id;

            // extract the response
            response = client.GetAsync(url).Result;

            IEnumerable<GenreDto> GenreDtos = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            // assing it to the view model
            DetailsGames.Genres = GenreDtos;


            // GET LIST OF GENRES NOT IN A GAME
            // fetch list of genres not included in a game
            url = "GenreData/GetAllGenresNotInGame/" + id;

            // extract the response
            response = client.GetAsync(url).Result;

            IEnumerable<GenreDto> AvailableGenres = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            // assing it to the view model
            DetailsGames.AvailableGenres = AvailableGenres;

            // fetch the list of activities for a particular game id
            url = "ActivityData/GetAllActivitiesForGame/" + id;

            // extract the response
            response = client.GetAsync(url).Result;

            IEnumerable<ActivityDto> Activities = response.Content.ReadAsAsync<IEnumerable<ActivityDto>>().Result;

            // assign it to the view model
            DetailsGames.Activities = Activities;

            // pass the data to the view
            return View(DetailsGames);
        }

        //POST: Game/Associate/{id}?GenreId={GenreId}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Associate(int id, int GenreId)
        {
            GetApplicationCookie();//get token credentials

            //call our api to associate game with genre
            string url = "GameData/AssociateGameWithGenre/" + id + "/" + GenreId;

            // append an empty string content as this is a post request without data
            HttpContent content = new StringContent("");

            // update the content-type property
            content.Headers.ContentType.MediaType = "application/json";

            // send the request and read the results
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //Get: Game/UnAssociate/{id}?GenreId={GenreId}
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult UnAssociate(int id, int GenreId)
        {
            GetApplicationCookie();//get token credentials

            //call our api to unassociate game with genre
            string url = "GameData/UnAssociateGameWithGenre/" + id + "/" + GenreId;

            // append an empty string content as this is a post request without data
            HttpContent content = new StringContent("");

            // update the content-type property
            content.Headers.ContentType.MediaType = "application/json";

            // send the request and read the results
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        // GET: Game/Add
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            return View();
        }

        // POST: Game/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Game Game)
        {
            GetApplicationCookie();//get token credentials

            // make a curl request to
            // https://localhost:44313/api/GameData/AddGame/4

            // generate the url string
            string url = "GameData/AddGame";

            // serialize the json payload
            string jsonpayload = jss.Serialize(Game);

            // create a new string content with the serialized json
            HttpContent content = new StringContent(jsonpayload);

            // update the content type to application/json
            content.Headers.ContentType.MediaType = "application/json";

            // get the response from the database
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            // check for status code
            // if success then go to list page
            // else go to the error page
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Game/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();//get token credentials

            // ge the details of the game from the given id
            // make a curl request to
            // https://localhost:44313/api/GameData/GetGameDetails/4

            // assign the url to a string
            string url = "GameData/GetGameDetails/" + id;

            // get the result
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create an empty DTO Object and read the data
            GameDto game = response.Content.ReadAsAsync<GameDto>().Result;

            ViewData["ReleaseDate"] = Convert.ToDateTime(game.GameReleaseDate).ToString("yyyy-MM-dd");

            // pass the data to the view
            return View(game);
        }

        // POST: Game/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Game Game)
        {
            try
            {
                GetApplicationCookie();//get token credentials

                // call the update activity api using the following api
                // curl -H "Content-Type:application/json" -d @game.json https://localhost:44313/api/GameData/UpdateGame/4
                string url = "GameData/UpdateGame/" + id;

                // serialize the json payload
                string jsonpayload = jss.Serialize(Game);

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

        // GET: Game/ConfirmDelete/5
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmDelete(int id)
        {
            // ge the details of the game from the given id
            // make a curl request to
            // https://localhost:44313/api/GameData/GetGameDetails/4

            // assign the url to a string
            string url = "GameData/GetGameDetails/" + id;

            // get the result
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create an empty DTO Object and read the data
            GameDto game = response.Content.ReadAsAsync<GameDto>().Result;

            // pass the data to the view
            return View(game);
        }

        // POST: Game/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                GetApplicationCookie();//get token credentials

                // call the delete game api
                // curl -d "" https://localhost:44313/api/GameData/DeleteGame/4
                string url = "GameData/DeleteGame/" + id;

                // as this is a delete request the string content will be an empty string
                HttpContent content = new StringContent("");

                // change the content type to application json
                content.Headers.ContentType.MediaType = "application/json";

                // read the data 
                HttpResponseMessage response = client.PostAsync(url, content).Result;

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