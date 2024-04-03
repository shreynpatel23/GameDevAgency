using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GameDevAgency.Models;

namespace GameDevAgency.Controllers
{
    public class GameDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/GameData/GetAllGames  
        [HttpGet]
        [Route("api/GameData/GetAllGames")]
        public IEnumerable<GameDto> GetAllGames()
        {
            List<Game> Games = db.Games.ToList();
            List<GameDto> GameDtos = new List<GameDto>();

            // loop through the games array and push it to gamedto
            Games.ForEach(game => GameDtos.Add(new GameDto()
            {
                GameId = game.GameId,
                GameName = game.GameName,
                GameVersion = game.GameVersion,
                GameDescription = game.GameDescription,
                GameReleaseDate = game.GameReleaseDate,
            }));

            // return update GameDto
            return GameDtos;
        }

        // GET: api/GameData/GetAllGamesForGenre/1  
        [HttpGet]
        [Route("api/GameData/GetAllGamesForGenre/{id}")]
        public IEnumerable<GameDto> GetAllGamesForGenre(int id)
        {
            List<Game> Games = db.Games.Where(
                game => game.Genres.Any(genre => genre.GenreId == id)
                ).ToList();
            List<GameDto> GameDtos = new List<GameDto>();

            // loop through the games array and push it to gamedto
            Games.ForEach(game => GameDtos.Add(new GameDto()
            {
                GameId = game.GameId,
                GameName = game.GameName,
                GameVersion = game.GameVersion,
                GameDescription = game.GameDescription,
                GameReleaseDate = game.GameReleaseDate,
            }));

            // return update GameDto
            return GameDtos;
        }

        // GET: api/GameData/GetGameDetails/2
        [ResponseType(typeof(Game))]
        [HttpGet]
        [Route("api/GameData/GetGameDetails/{id}")]
        public IHttpActionResult GetGameDetails(int id)
        {
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        // PUT: api/GameData/UpdateGame/2
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/GameData/UpdateGame/{id}")]
        public IHttpActionResult UpdateGame(int id, Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != game.GameId)
            {
                return BadRequest();
            }

            db.Entry(game).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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

        // POST: api/GameData/AddGame
        [ResponseType(typeof(Game))]
        [Route("api/GameData/AddGame")]
        [HttpPost]
        public IHttpActionResult AddGame(Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Games.Add(game);
            db.SaveChanges();

            return Ok();
        }

        // DELETE: api/GameData/DeleteGame/2
        [ResponseType(typeof(Game))]
        [HttpPost]
        [Route("api/GameData/DeleteGame/{id}")]
        public IHttpActionResult DeleteGame(int id)
        {
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return NotFound();
            }

            db.Games.Remove(game);
            db.SaveChanges();

            return Ok(game);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameExists(int id)
        {
            return db.Games.Count(e => e.GameId == id) > 0;
        }
    }
}