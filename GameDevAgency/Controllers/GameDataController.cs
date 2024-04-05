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

        /// <summary>
        /// Returns a list of all the games present in the database. Here as
        /// we have a foreign key we need to return GamesDto.
        /// </summary>
        /// <example>GET api/GameData/GetAllGames</example>
        /// <example>
        /// GET: curl "http://localhost:50860/api/GameData/GetAllGames"
        /// </example>
        /// <returns>
        /// A list of all the games DTO (Data trasnferable object) present in DB.
        /// </returns>
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


        /// <summary>
        /// Retrieves all games belonging to a specific genre from the database.
        /// </summary>
        /// <param name="id">The genre ID.</param>
        /// <returns>An enumerable collection of GameDto objects.</returns>
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

        /// <summary>
        /// Associates a game with a genre.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="genreId">The ID of the genre.</param>
        /// <returns>An IHttpActionResult indicating the status of the association operation.</returns>
        [HttpPost]
        [Route("api/GameData/AssociateGameWithGenre/{gameId}/{genreId}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AssociateGameWithGenre(int gameId, int genreId)
        {

            Game Game = db.Games.Include(game => game.Genres).Where(game => game.GameId == gameId).FirstOrDefault();
            Genre Genre = db.Genres.Find(genreId);

            if (Game == null || Genre == null)
            {
                return NotFound();
            }


            Game.Genres.Add(Genre);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Unassociates a game with a genre.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="genreId">The ID of the genre.</param>
        /// <returns>An IHttpActionResult indicating the status of the unassociation operation.</returns>
        [HttpPost]
        [Route("api/GameData/UnAssociateGameWithGenre/{gameId}/{genreId}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UnAssociateGameWithGenre(int gameId, int genreId)
        {

            Game Game = db.Games.Include(game => game.Genres).Where(game => game.GameId == gameId).FirstOrDefault();
            Genre Genre = db.Genres.Find(genreId);

            if (Game == null || Genre == null)
            {
                return NotFound();
            }


            Game.Genres.Remove(Genre);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns details of a particular game
        /// </summary>
        /// <param name="id">the id of game to fetch the details of it</param>
        /// <example>GET api/GameData/GetGameDetails/2</example>
        /// <example>
        /// GET: curl "http://localhost:50860/api/GameData/GetGameDetails/1"
        /// </example>
        /// <returns>
        /// A single activity DTO (data transferable object) with data
        /// </returns>
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

            // create a new game dto object and store the values in it
            GameDto GameDto = new GameDto()
            {
                GameId = game.GameId,
                GameName = game.GameName,
                GameVersion = game.GameVersion,
                GameDescription = game.GameDescription,
                GameReleaseDate = game.GameReleaseDate,
            };


            // return update GameDto
            return Ok(GameDto);
        }

        /// <summary>
        /// Updates details of a specific game in the database.
        /// </summary>
        /// <param name="id">The game ID.</param>
        /// <param name="game">The updated game object.</param>
        /// <returns>An IHttpActionResult indicating the status of the update operation.</returns>
        // PUT: api/GameData/UpdateGame/2
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/GameData/UpdateGame/{id}")]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Adds a new game to the database.
        /// </summary>
        /// <param name="game">The game object to be added.</param>
        /// <returns>An IHttpActionResult indicating the status of the add operation.</returns>
        // POST: api/GameData/AddGame
        [ResponseType(typeof(Game))]
        [Route("api/GameData/AddGame")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Deletes a specific game from the database.
        /// </summary>
        /// <param name="id">The game ID.</param>
        /// <returns>An IHttpActionResult containing the deleted game details.</returns>
        // DELETE: api/GameData/DeleteGame/2
        [ResponseType(typeof(Game))]
        [HttpPost]
        [Route("api/GameData/DeleteGame/{id}")]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Disposes the database context.
        /// </summary>
        /// <param name="disposing">A flag indicating whether to dispose managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks if a game exists in the database.
        /// </summary>
        /// <param name="id">The game ID.</param>
        /// <returns>True if the game exists; otherwise, false.</returns>
        private bool GameExists(int id)
        {
            return db.Games.Count(e => e.GameId == id) > 0;
        }
    }
}