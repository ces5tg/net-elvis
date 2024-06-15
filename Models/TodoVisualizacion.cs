using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TodoApi.Models
{
    public class Visualizacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string IdUsuario { get; set; } = default!;

        public string IdMovie { get; set; } = default!;

        public TimeSpan TimeVisualizacion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public DateTime FechaVisualizacion { get; set; } = DateTime.UtcNow;
    }
}
