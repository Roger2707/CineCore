using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SearchService.Entities
{
    public class MovieSearch
    {
        [BsonId] 
        [BsonRepresentation(BsonType.String)] 
        public Guid Id { get; set; }

        [BsonElement("title")] 
        public string Title { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("durationMinutes")]
        public int DurationMinutes { get; set; }

        [BsonElement("posterUrl")]
        public string PosterUrl { get; set; } = null!;

        [BsonElement("publicId")]
        public string PublicId { get; set; } = null!;

        [BsonElement("genres")]
        public List<string> Genres { get; set; } = new List<string>();
    }
}
