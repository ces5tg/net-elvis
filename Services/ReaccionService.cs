using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class ReaccionService : IReaccionService
    {
        private readonly IMongoCollection<Reaccion> _reacciones;
        private readonly IMongoCollection<Movie> _movies;

        public ReaccionService(IOptions<TodoDatabaseSettings> settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _reacciones = database.GetCollection<Reaccion>(settings.Value.ReaccionCollectionName);
            _movies = database.GetCollection<Movie>(settings.Value.MoviesCollectionName);
        }

        public async Task<List<Reaccion>> GetAsync() =>
            await _reacciones.Find(_ => true).ToListAsync();

        public async Task<Reaccion?> GetAsync(string id) =>
            await _reacciones.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Reaccion reaccion)
        {
            // Calcula la sumatoria antes de insertar
            reaccion.Sumatoria = Convert.ToInt32(reaccion.Like) +
                                 Convert.ToInt32(reaccion.Comentario) +
                                 Convert.ToInt32(reaccion.Compartir) +
                                 Convert.ToInt32(reaccion.Repetir);

            await _reacciones.InsertOneAsync(reaccion);

            // Actualiza los likes/dislikes en la película
            await UpdateMovieLikes(reaccion.IdMovie, reaccion.Like ? 1 : -1);
        }

        public async Task UpdateAsync(string id, Reaccion updatedReaccion)
        {
            // Calcula la sumatoria antes de actualizar
            updatedReaccion.Sumatoria = Convert.ToInt32(updatedReaccion.Like) +
                                        Convert.ToInt32(updatedReaccion.Comentario) +
                                        Convert.ToInt32(updatedReaccion.Compartir) +
                                        Convert.ToInt32(updatedReaccion.Repetir);

            var existingReaccion = await GetAsync(id);
            if (existingReaccion != null)
            {
                await _reacciones.ReplaceOneAsync(r => r.Id == id, updatedReaccion);

                // Actualiza los likes/dislikes en la película solo si cambió el estado de Like
                if (existingReaccion.Like != updatedReaccion.Like)
                {
                    int incrementLikes = updatedReaccion.Like ? 1 : -1;
                    await UpdateMovieLikes(updatedReaccion.IdMovie, incrementLikes);
                }
            }
        }

        public async Task RemoveAsync(string id)
        {
            var existingReaccion = await GetAsync(id);
            if (existingReaccion != null)
            {
                await _reacciones.DeleteOneAsync(r => r.Id == id);

                // Actualiza los likes/dislikes en la película si se estaba eliminando un like
                if (existingReaccion.Like)
                {
                    await UpdateMovieLikes(existingReaccion.IdMovie, -1);
                }
            }
        }

        private async Task UpdateMovieLikes(string movieId, int incrementLikes)
        {
            var filter = Builders<Movie>.Filter.Eq(m => m.MovieId, movieId);
            var update = Builders<Movie>.Update.Inc(m => m.Likes, incrementLikes);

            await _movies.UpdateOneAsync(filter, update);
        }
    }

    public interface IReaccionService
    {
        Task<List<Reaccion>> GetAsync();
        Task<Reaccion?> GetAsync(string id);
        Task CreateAsync(Reaccion reaccion);
        Task UpdateAsync(string id, Reaccion updatedReaccion);
        Task RemoveAsync(string id);
    }
}
