using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TodoApi.Models
{
    public class Reaccion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string IdUsuario { get; set; }

        public string IdMovie { get; set; }

        public bool Like { get; set; }
        public bool Comentario { get; set; }
        public bool Compartir { get; set; }
        public bool Repetir { get; set; }

        public int Sumatoria { get; set; } // Suma total de reacciones

        public string ComentarioTexto { get; set; } // Comentario de la reacción

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
