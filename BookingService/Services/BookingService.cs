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

        #region Booking Handle

        public async Task Create(BookingCreateRequestDTO request)
        {
            if(IsShowExisted(request.ScreeningId) == false)
                throw new ArgumentException("Screening Is Not Found");

            if(request.Seats.Count == 0)
                throw new ArgumentException("Please choose seats !");

            var bookingId = request.BookingId;
            await _bookingRepository.BeginTransactionAsync();
            try
            {
                #region Create 

                var booking = new Booking
                {
                    Id = bookingId,
                    UserId = request.UserId,
                    ScreeningId = request.ScreeningId,
                    TotalPrice = request.Seats.Count * 139000000,
                    Created = DateTime.UtcNow,
                    BookingStatus = BookingStatus.CONFIRMED,
                    PaymentIntentId = request.PaymentIntentId,
                };
                await _bookingRepository.Create(booking);

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
                _logger.LogInformation("Booking created successfully: {BookingId}", booking.Id);

                await _bookingRepository.SaveChangeAsync();
                await _bookingRepository.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create booking: {BookingId}", bookingId);
                await _bookingRepository.RollbackTransactionAsync();
                throw;
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
            return screening != null;
        }

        #endregion
    }
}
