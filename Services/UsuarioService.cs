using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TodoApi.Models
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuarios;

        public UsuarioService(IOptions<TodoDatabaseSettings> settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _usuarios = database.GetCollection<Usuario>(settings.Value.UsuariosCollectionName);
        }

        public async Task<List<Usuario>> GetAsync() =>
            await _usuarios.Find(usuario => true).ToListAsync();

        public async Task<Usuario?> GetAsync(string id) =>
            await _usuarios.Find(usuario => usuario.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Usuario usuario)
        {
            // Validar modelo antes de insertar
            if (ValidateModel(usuario))
                await _usuarios.InsertOneAsync(usuario);
        }

        public async Task UpdateAsync(string id, Usuario updatedUsuario)
        {
            // Validar modelo antes de actualizar
            if (ValidateModel(updatedUsuario))
                await _usuarios.ReplaceOneAsync(usuario => usuario.Id == id, updatedUsuario);
        }

        public async Task RemoveAsync(string id) =>
            await _usuarios.DeleteOneAsync(usuario => usuario.Id == id);

        public async Task<Usuario?> AuthenticateAsync(string gmail, string password)
        {
            var usuario = await _usuarios.Find(u => u.Gmail == gmail && u.Password == password).FirstOrDefaultAsync();
            return usuario;
        }

        // Método para validar el modelo Usuario
        private bool ValidateModel(Usuario usuario)
        {
            var context = new ValidationContext(usuario, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(usuario, context, results, validateAllProperties: true);

            if (!isValid)
            {
                string errorMessages = string.Join("\n", results.Select(r => r.ErrorMessage));
                // Log or handle validation errors
                return false;
            }

            return true;
        }
    }

    public interface IUsuarioService
    {
        Task<List<Usuario>> GetAsync();
        Task<Usuario?> GetAsync(string id);
        Task CreateAsync(Usuario usuario);
        Task UpdateAsync(string id, Usuario updatedUsuario);
        Task RemoveAsync(string id);
        Task<Usuario?> AuthenticateAsync(string gmail, string password);
    }
}
