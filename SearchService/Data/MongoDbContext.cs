using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService.DB;

namespace SearchService.Data
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IOptions<MongoDbSettings> settings, IMongoClient client)
        {
            _db = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }
}
