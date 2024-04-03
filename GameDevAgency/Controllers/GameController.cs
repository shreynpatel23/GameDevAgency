using GameDevAgency.Models;
using System;
using System.Collections.Generic;
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

        // error page
        // GET  Error
        public ActionResult Error()
        {

            return View();
        }

        // GET: Game/List
        public ActionResult List()
        {
            // make a curl request to
            // https://localhost:44313/api/GameData/GetAllGames

            // create a url to call api
            string url = "GameData/GetAllGames";

            // get the response 
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create empty GameDta list and read the data from the response
            IEnumerable<GameDto> games = response.Content.ReadAsAsync<IEnumerable<GameDto>>().Result;


            // send the data in the view
            return View(games);
        }

        // GET: Game/Details/5
        public ActionResult Details(int id)
        {
            // make a curl request to
            // https://localhost:44313/api/GameData/GetGameDetails/4

            // assign the url to a string
            string url = "GameData/GetGameDetails/" + id;

            // use the string to get the response
            HttpResponseMessage response = client.GetAsync(url).Result;


            // create an empty DTO Object and read the data
            GameDto game = response.Content.ReadAsAsync<GameDto>().Result;

            // pass the data to the view
            return View(game);
        }

        // GET: Game/Add
        public ActionResult Add()
        {
            return View();
        }

        // POST: Game/Create
        [HttpPost]
        public ActionResult Create(Game Game)
        {
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
        public ActionResult Edit(int id)
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

            ViewData["ReleaseDate"] = Convert.ToDateTime(game.GameReleaseDate).ToString("yyyy-MM-dd");

            // pass the data to the view
            return View(game);
        }

        // POST: Game/Update/5
        [HttpPost]
        public ActionResult Update(int id, Game Game)
        {
            try
            {
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
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
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