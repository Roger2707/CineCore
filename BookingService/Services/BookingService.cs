using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories.IRepositories;
using BookingService.Services.IServices;
using Contracts.BookingEvents;
using StackExchange.Redis;

namespace BookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly GrpcScreeningClientService _grpcScreeningClientService;
        private readonly IDatabase _redis;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IBookingRepository bookingRepository, GrpcScreeningClientService grpcScreeningClientService
            , IConnectionMultiplexer redis, ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _grpcScreeningClientService = grpcScreeningClientService;
            _redis = redis.GetDatabase();
            _logger = logger;
        }

        #region GET
        public async Task<List<BookingDTO>> GetBookings()
        {
            return await _bookingRepository.GetBookings();
        }

        public async Task<BookingDTO> GetBooking(Guid bookingId)
        {
            var booking = await _bookingRepository.GetBooking(bookingId);   
            if(booking == null) throw new ArgumentException("Booking Not Found");    
            return booking;
        }

        #endregion

        #region Booking Flow

        public async Task Create(BookingCreateRequestDTO request)
        {
            if(IsShowExisted(request.ScreeningId) == false)
                throw new ArgumentException("Show Not Found");

            if(request.Seats.Count == 0)
                throw new ArgumentException("Please choose seats !");

            await _bookingRepository.BeginTransactionAsync();
            try
            {
                #region Race Condition

                foreach (var seat in request.Seats)
                {
                    var key = $"seat:{seat}:screen:{request.ScreeningId}";
                    var existingValue = await _redis.StringGetAsync(key);
                    if (existingValue != RedisValue.Null && existingValue != request.UserId.ToString())
                        throw new ArgumentException($"Seat {seat} is not available");

                    var held = await _redis.StringSetAsync(key, request.UserId.ToString(), TimeSpan.FromSeconds(10), When.NotExists);
                    if (!held)
                        throw new ArgumentException($"Seat {seat} is not held successfully ! someone is held");
                }

                #endregion

                #region Create 

                var booking = new Booking
                { 
                    UserId = request.UserId,
                    ScreeningId = request.ScreeningId,
                    TotalPrice = request.Seats.Count * 139000000,
                    Created = DateTime.UtcNow,
                    BookingStatus = BookingStatus.PENDING
                };
                await _bookingRepository.Create(booking);
                await _bookingRepository.SaveChangeAsync();

                var bookingSeats = request.Seats.Select(s =>
                {
                    return new BookingSeat
                    {
                        BookingId = booking.Id,
                        SeatId = s,
                        Price = 139000000,
                        Created = DateTime.UtcNow
                    };
                }).ToList();

                await _bookingRepository.Create(bookingSeats);

                #endregion

                await _bookingRepository.PublishMessageAsync(new BookingCreated
                {
                    BookingId = booking.Id,
                    SeatIds = request.Seats,
                    ScreeningId = request.ScreeningId
                });
                _logger.LogInformation("Published BookingCreated event for {BookingId}", booking.Id);

                await _bookingRepository.SaveChangeAsync();
                await _bookingRepository.CommitTransactionAsync();
            }
            catch (ArgumentException)
            {
                await _bookingRepository.RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish BookingCreated event");
                await _bookingRepository.RollbackTransactionAsync();
                throw new Exception("Error when creating booking", ex);
            }
        }

        public async Task Delete(Guid bookingId, List<Guid> SeatIds, Guid ScreeningId)
        {
            // 1. Delete from db
            await _bookingRepository.Delete(bookingId);

            // 2. Delete from redis
            var tasks = SeatIds.Select(async s =>
            {
                var key = $"seat:{s}:screen:{ScreeningId}";
                await _redis.KeyDeleteAsync(key);
            });
            await Task.WhenAll(tasks);
            await _bookingRepository.SaveChangeAsync();
        }

        #endregion

        #region Validations

        private bool IsShowExisted(Guid screeningId)
        {
            var screening = _grpcScreeningClientService.GetScreening(screeningId);
            if (screening == null) throw new ArgumentException("Show Not Found");
            return true;
        }

        #endregion
    }
}
