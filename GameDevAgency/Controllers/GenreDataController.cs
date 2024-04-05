using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web.Http;
using System.Web.Http.Description;
using GameDevAgency.Models;

namespace GameDevAgency.Controllers
{
    public class GenreDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns a list of all the genres present in the database. Here as
        /// we have a foreign key we need to return GenreDto.
        /// </summary>
        /// <example>GET api/GenreData/GetAllGenres</example>
        /// <example>
        /// GET: curl "http://localhost:50860/api/GenreData/GetAllGenres"
        /// </example>
        /// <returns>
        /// A list of all the genres DTO (Data trasnferable object) present in DB.
        /// </returns>
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

        /// <summary>
        /// Retrieves all genres associated with a specific game.
        /// </summary>
        /// <param name="GameId">The ID of the game.</param>
        /// <returns>An IHttpActionResult containing a list of GenreDto objects.</returns>
        // GET: api/GenreData/GetAllGenresForGame/{GameId}
        [HttpGet]
        [Route("api/GenreData/GetAllGenresForGame/{GameId}")]
        public IHttpActionResult GetAllGenresForGame(int GameId)
        {
            List<Genre> Genres = db.Genres.Where(
                genre => genre.Games.Any(
                    game => game.GameId == GameId)
                ).ToList();
            List<GenreDto> GenreDto = new List<GenreDto>();

            Genres.ForEach(genre => GenreDto.Add(new GenreDto()
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName,
            }));

            return Ok(GenreDto);
        }

        /// <summary>
        /// Retrieves all genres not associated with a specific game.
        /// </summary>
        /// <param name="GameId">The ID of the game.</param>
        /// <returns>An IHttpActionResult containing a list of GenreDto objects.</returns>
        // GET: api/GenreData/GetAllGenresNotInGame/{GameId}
        [HttpGet]
        [Route("api/GenreData/GetAllGenresNotInGame/{GameId}")]
        public IHttpActionResult GetAllGenresNotInGame(int GameId)
        {
            List<Genre> Genres = db.Genres.Where(
                genre => !genre.Games.Any(
                    game => game.GameId == GameId)
                ).ToList();
            List<GenreDto> GenreDto = new List<GenreDto>();

            Genres.ForEach(genre => GenreDto.Add(new GenreDto()
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName,
            }));

            return Ok(GenreDto);
        }

        /// <summary>
        /// Returns details of a particular genre
        /// </summary>
        /// <param name="id">the id of genre to fetch the details of it</param>
        /// <example>GET api/GenreData/GetGenreDetails/2</example>
        /// <example>
        /// GET: curl "http://localhost:50860/api/GenreData/GetGenreDetails/1"
        /// </example>
        /// <returns>
        /// A single activity DTO (data transferable object) with data
        /// </returns>
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

        /// <summary>
        /// Updates details of a specific genre in the database.
        /// </summary>
        /// <param name="id">The ID of the genre.</param>
        /// <param name="genre">The updated genre object.</param>
        /// <returns>An IHttpActionResult indicating the status of the update operation.</returns>
        // PUT: api/GenreData/UpdateGenre/2
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/GenreData/UpdateGenre/{id}")]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Adds a new genre to the database.
        /// </summary>
        /// <param name="genre">The genre object to be added.</param>
        /// <returns>An IHttpActionResult indicating the status of the add operation.</returns>
        // POST: api/GenreData/AddGenre
        [ResponseType(typeof(Genre))]
        [HttpPost]
        [Route("api/GenreData/AddGenre")]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Deletes a specific genre from the database.
        /// </summary>
        /// <param name="id">The ID of the genre.</param>
        /// <returns>An IHttpActionResult containing the deleted genre details.</returns>
        // DELETE: api/GenreData/DeleteGenre/2
        [ResponseType(typeof(Genre))]
        [HttpPost]
        [Route("api/GenreData/DeleteGenre/{id}")]
        [Authorize(Roles = "Admin")]
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