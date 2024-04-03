using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using System.Web.Http.Description;
using GameDevAgency.Models;

namespace GameDevAgency.Controllers
{
    public class GenreDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/GenreData/GetAllGenres
        [HttpGet]
        [Route("api/GenreData/GetAllGenres")]
        public IEnumerable<GenreDto> GetAllGenres()
        {

            List<Genre> Genres = db.Genres.ToList();

            List<GenreDto> GenreDtos = new List<GenreDto>();

            // loop through the Genre array and push it to genredto
            Genres.ForEach(genre => GenreDtos.Add(new GenreDto()
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName,
            }));

            // return the data
            return GenreDtos;
        }

        // GET: api/GenreData/GetGenreDetails/2
        [ResponseType(typeof(Genre))]
        [HttpGet]
        [Route("api/GenreData/GetGenreDetails/{id}")]
        public IHttpActionResult GetGenre(int id)
        {
            Genre genre = db.Genres.Find(id);
            if (genre == null)
            {
                return NotFound();
            }


            // create a new game dto object and store the values in it
            GenreDto GenreDto = new GenreDto()
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName,
            };


            // return update GameDto
            return Ok(GenreDto);
        }

        // PUT: api/GenreData/UpdateGenre/2
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/GenreData/UpdateGenre/{id}")]
        public IHttpActionResult PutGenre(int id, Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != genre.GenreId)
            {
                return BadRequest();
            }

            db.Entry(genre).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/GenreData/AddGenre
        [ResponseType(typeof(Genre))]
        [HttpPost]
        [Route("api/GenreData/AddGenre")]
        public IHttpActionResult PostGenre(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Genres.Add(genre);
            db.SaveChanges();

            return Ok();
        }

        // DELETE: api/GenreData/DeleteGenre/2
        [ResponseType(typeof(Genre))]
        [HttpPost]
        [Route("api/GenreData/DeleteGenre/{id}")]
        public IHttpActionResult DeleteGenre(int id)
        {
            Genre genre = db.Genres.Find(id);
            if (genre == null)
            {
                return NotFound();
            }

            db.Genres.Remove(genre);
            db.SaveChanges();

            return Ok(genre);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GenreExists(int id)
        {
            return db.Genres.Count(e => e.GenreId == id) > 0;
        }
    }
}