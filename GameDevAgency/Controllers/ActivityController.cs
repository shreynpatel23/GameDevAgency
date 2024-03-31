using GameDevAgency.Models;
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
        } // renders the error page
        // GET  Error
        public ActionResult Error()
        {

            return View();
        }

        // renders the activity list page
        // GET: Activity/List
        public ActionResult List()
        {
            // other way to fetch the data is 
            //curl https://localhost:44313/api/ActivityData/GetAllActivities

            // create a url to call api
            string url = "ActivityData/GetAllActivities";

            // get the response 
            HttpResponseMessage response = client.GetAsync(url).Result;

            // create empty activityDTO list and read the data from the response
            IEnumerable<ActivityDto> activities = response.Content.ReadAsAsync<IEnumerable<ActivityDto>>().Result;


            // send the data in the view
            return View(activities);
        }

        // GET: Activity/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Activity/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activity/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Activity/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Activity/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Activity/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Activity/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
