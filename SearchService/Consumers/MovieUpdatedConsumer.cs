using Contracts;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using P3.SearchService.Data;
using P3.SearchService.Entities;

namespace P3.SearchService.Consumers
{
    public class MovieUpdatedConsumer : IConsumer<MovieUpdated>
    {
        private readonly IMongoCollection<MovieSearch> _collection;
        public MovieUpdatedConsumer(IMongoDbContext context, IOptions<MongoDbSettings> settings)
        {
            _collection = context.GetCollection<MovieSearch>(settings.Value.MovieCollection);
        }
        public async Task Consume(ConsumeContext<MovieUpdated> context)
        {
            Console.WriteLine($"---> Consuming Movie update process id: {context.Message.Id}, title: {context.Message.Title}");

            var update = Builders<MovieSearch>.Update
                    .Set(x => x.Title, context.Message.Title)
                    .Set(x => x.Description, context.Message.Description)
                    .Set(x => x.DurationMinutes, context.Message.DurationMinutes)
                    .Set(x => x.PosterUrl, context.Message.PosterUrl)
                    .Set(x => x.PublicId, context.Message.PublicId)
                    .Set(x => x.Genres, context.Message.Genres);

            var result = await _collection.UpdateOneAsync(
                x => x.Id == context.Message.Id,
                update
            );

            if (result.MatchedCount == 0) Console.WriteLine("No matching movie found to update.");
            else Console.WriteLine("Movie updated successfully.");
        }
    }
}
