using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _movieService.GetAsync();
            return Ok(movies);
        }

        // GET: api/Movies/5
        [HttpGet("{id:length(24)}", Name = "GetMovie")]
        public async Task<ActionResult<Movie>> GetMovie(string id)
        {
            var movie = await _movieService.GetAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        // POST: api/Movies
        // POST: api/Movies
[HttpPost]
public async Task<ActionResult<Movie>> PostMovie(Movie movie)
{
    try
    {
        // Intenta insertar la película
        await _movieService.CreateAsync(movie);

        // Verifica si la inserción fue exitosa
        var insertedMovie = await _movieService.GetAsync(movie.Id);
        if (insertedMovie == null)
        {
            return StatusCode(500, "Error: La película no se ha insertado correctamente en la base de datos.");
        }

        // Si todo está bien, devuelve una respuesta exitosa junto con la película insertada
        return CreatedAtRoute("GetMovie", new { id = insertedMovie.Id }, insertedMovie);
    }
    catch (Exception ex)
    {
        // Captura cualquier excepción y devuelve un mensaje de error
        return StatusCode(500, $"Error al insertar la película: {ex.Message}");
    }
}


        // PUT: api/Movies/5
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> PutMovie(string id, Movie updatedMovie)
        {
            var movie = await _movieService.GetAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            updatedMovie.Id = movie.Id;
            await _movieService.UpdateAsync(id, updatedMovie);

            return NoContent();
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteMovie(string id)
        {
            var movie = await _movieService.GetAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            await _movieService.RemoveAsync(id);
            return NoContent();
        }
    }
}
