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
    public class GenreController : Controller
    {
        // declare http client and js serializer
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        // create a new client and configure the base url
        static GenreController()
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

        // GET: Genre/List
        public ActionResult List()
        {
            // declare the view model
            ListGenres ListGenres = new ListGenres();

            // check if the user role is admin or not
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ListGenres.IsAdmin = true;
            else ListGenres.IsAdmin = false;

            // make a curl request to
            // https://localhost:44313/api/GenreData/GetAllGenres

            // create a url to call api
            string url = "GenreData/GetAllGenres";

            // get the response 
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create empty GenreDto list and read the data from the response
            IEnumerable<GenreDto> genres = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            // append the data in the view model
            ListGenres.Genres = genres;

            // send the data in the view
            return View(ListGenres);
        }

        // GET: Genre/Details/5
        public ActionResult Details(int id)
        {
            // view model to get genre details and list of games for each genre
            DetailsGenre DetailsGenre = new DetailsGenre();

            // check if the user role is admin or not
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) DetailsGenre.IsAdmin = true;
            else DetailsGenre.IsAdmin = false;

            // make a curl request to get the genre details
            // https://localhost:44313/api/GenreData/GetGenreDetails/4

            // assign the url to a string
            string url = "GenreData/GetGenreDetails/" + id;

            // use the string to get the response
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create an empty DTO Object and read the data
            GenreDto genreDto = response.Content.ReadAsAsync<GenreDto>().Result;

            // assign the details to view model
            DetailsGenre.Genre = genreDto;

            // make a curl request to get the list of games for genre id
            // https://localhost:44313/api/GameData/GetAllGamesForGenre/4

            // assign the url to a string
            url = "GameData/GetAllGamesForGenre/" + id;

            // use the string to get the response
            response = client.GetAsync(url).Result;

            // create an empty DTO Object and read the data
            IEnumerable<GameDto> Games = response.Content.ReadAsAsync<IEnumerable<GameDto>>().Result;

            // assign the list of games to view model
            DetailsGenre.Games = Games;

            // pass the data to the view
            return View(DetailsGenre);
        }

        // GET: Genre/Add
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            return View();
        }

        // POST: Genre/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Genre Genre)
        {
            GetApplicationCookie();//get token credentials

            // make a curl request to
            // https://localhost:44313/api/GenreData/AddGenre/4

            // generate the url string
            string url = "GenreData/AddGenre";

            // serialize the json payload
            string jsonpayload = jss.Serialize(Genre);

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

        // GET: Genre/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            // make a curl request to
            // https://localhost:44313/api/GenreData/GetGenreDetails/4

            // assign the url to a string
            string url = "GenreData/GetGenreDetails/" + id;

            // use the string to get the response
            HttpResponseMessage response = client.GetAsync(url).Result;


            // create an empty DTO Object and read the data
            GenreDto genreDto = response.Content.ReadAsAsync<GenreDto>().Result;

            // pass the data to the view
            return View(genreDto);
        }

        // POST: Genre/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Genre Genre)
        {
            try
            {
                GetApplicationCookie();//get token credentials

                // call the update activity api using the following api
                // curl -H "Content-Type:application/json" -d @genre.json https://localhost:44313/api/GenreData/UpdateGenre/4
                string url = "GenreData/UpdateGenre/" + id;

                // serialize the json payload
                string jsonpayload = jss.Serialize(Genre);

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

        // GET: Genre/ConfirmDelete/5
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmDelete(int id)
        {
            // make a curl request to
            // https://localhost:44313/api/GenreData/GetGenreDetails/4

            // assign the url to a string
            string url = "GenreData/GetGenreDetails/" + id;

            // use the string to get the response
            HttpResponseMessage response = client.GetAsync(url).Result;


            // create an empty DTO Object and read the data
            GenreDto genreDto = response.Content.ReadAsAsync<GenreDto>().Result;

            // pass the data to the view
            return View(genreDto);
        }

        // POST: Genre/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                GetApplicationCookie();//get token credentials

                // call the delete game api
                // curl -d "" https://localhost:44313/api/GenreData/DeleteGenre/4
                string url = "GenreData/DeleteGenre/" + id;

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