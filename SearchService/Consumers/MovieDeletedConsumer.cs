using Contracts;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService.Data;
using SearchService.DB;
using SearchService.Entities;

namespace SearchService.Consumers
{
    public class MovieDeletedConsumer : IConsumer<MovieDeleted>
    {
        private readonly IMongoCollection<MovieSearch> _collection;
        public MovieDeletedConsumer(IMongoDbContext context, IOptions<MongoDbSettings> settings)
        {
            _collection = context.GetCollection<MovieSearch>(settings.Value.MovieCollection);
        }
        public async Task Consume(ConsumeContext<MovieDeleted> context)
        {
            Console.WriteLine($"---> Consuming Delete Movie id : {context.Message.Id}");
            var result = await _collection.DeleteOneAsync(x => x.Id == context.Message.Id);

            if (result.DeletedCount == 0) Console.WriteLine("No movie found to delete.");
            else Console.WriteLine("Movie deleted successfully.");
        }
    }
}
