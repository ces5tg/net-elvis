using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class VisualizacionService : IVisualizacionService
    {
        private readonly IMongoCollection<Visualizacion> _visualizaciones;
        private readonly IMongoCollection<Movie> _movies;
        private readonly IMongoCollection<Usuario> _usuarios;
        private readonly ILogger<VisualizacionService> _logger;

        public VisualizacionService(IOptions<TodoDatabaseSettings> settings, IMongoClient client, ILogger<VisualizacionService> logger)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _visualizaciones = database.GetCollection<Visualizacion>(settings.Value.VisualizacionCollectionName);
            _movies = database.GetCollection<Movie>(settings.Value.MoviesCollectionName);
            _usuarios = database.GetCollection<Usuario>("Usuarios");
            _logger = logger;
        }

        public async Task<Usuario> GetUsuarioAsync(string idUsuario)
        {
            var filter = Builders<Usuario>.Filter.Eq(u => u.Id, idUsuario);
            return await _usuarios.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Visualizacion>> GetAsync() =>
            await _visualizaciones.Find(_ => true).ToListAsync();

        public async Task<Visualizacion?> GetAsync(string id) =>
            await _visualizaciones.Find(v => v.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Visualizacion visualizacion)
        {
            // Elimina la asignación de Id, para que MongoDB lo genere automáticamente
            await _visualizaciones.InsertOneAsync(visualizacion);

            // Actualiza las estadísticas de la película
            await UpdateMovieStatsAsync(visualizacion);
        }

        public async Task UpdateAsync(string id, Visualizacion updatedVisualizacion)
        {
            await _visualizaciones.ReplaceOneAsync(v => v.Id == id, updatedVisualizacion);
            await UpdateMovieStatsAsync(updatedVisualizacion);
        }

        public async Task RemoveAsync(string id) =>
            await _visualizaciones.DeleteOneAsync(v => v.Id == id);

        private async Task UpdateMovieStatsAsync(Visualizacion visualizacion)
        {
            var filter = Builders<Movie>.Filter.Eq(m => m.MovieId, visualizacion.IdMovie);
            var update = Builders<Movie>.Update
                .Inc(m => m.Views, 1);

            await _movies.UpdateOneAsync(filter, update);
        }
    }

    public interface IVisualizacionService
    {
        Task<List<Visualizacion>> GetAsync();
        Task<Visualizacion?> GetAsync(string id);
        Task CreateAsync(Visualizacion visualizacion);
        Task UpdateAsync(string id, Visualizacion updatedVisualizacion);
        Task RemoveAsync(string id);
        Task<Usuario> GetUsuarioAsync(string idUsuario);
    }
}
