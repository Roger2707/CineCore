using MongoDB.Driver;

namespace P3.SearchService.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
