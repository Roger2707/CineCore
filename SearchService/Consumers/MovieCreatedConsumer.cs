using Contracts;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService.Data;
using SearchService.DB;
using SearchService.Entities;

namespace SearchService.Consumers
{
    public class MovieCreatedConsumer : IConsumer<MovieCreated>
    {
        private readonly IMongoCollection<MovieSearch> _collection;
        public MovieCreatedConsumer(IMongoDbContext context, IOptions<MongoDbSettings> settings)
        {
            _collection = context.GetCollection<MovieSearch>(settings.Value.MovieCollection);
        }

        public async Task Consume(ConsumeContext<MovieCreated> context)
        {
            Console.WriteLine($"Consuming movie created: {context.Message.Id} - {context.Message.Title}");

            await _collection.InsertOneAsync(new MovieSearch
            {
                Id = context.Message.Id,
                Title = context.Message.Title,
                Description = context.Message.Description,
                DurationMinutes = context.Message.DurationMinutes,
                PosterUrl = context.Message.PosterUrl,
                PublicId = context.Message.PublicId,
                Genres = context.Message.Genres
            });
        }
    }
}
