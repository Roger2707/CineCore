using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories.IRepositories;
using BookingService.Services.IServices;
using Contracts.BookingEvents;
using MassTransit;

namespace BookingService.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly GrpcScreeningClientService _grpcScreeningClientService;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookingService(IBookingRepository bookingRepository, GrpcScreeningClientService grpcScreeningClientService, IPublishEndpoint publishEndpoint)
        {
            _bookingRepository = bookingRepository;
            _grpcScreeningClientService = grpcScreeningClientService;
            _publishEndpoint = publishEndpoint;
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

            var booking = new Booking
            {
                UserId = request.UserId,
                ScreeningId = request.ScreeningId,
                TotalPrice = request.Seats.Count * 139000000,
                Created = DateTime.UtcNow,
                BookingStatus = BookingStatus.PENDING
            };

            var bookingSeats = new List<BookingSeat>();
            request.Seats.ForEach(s =>
            {
                bookingSeats.Add(new BookingSeat
                {
                    BookingId = booking.Id,
                    SeatId = s,
                    Price = 139000000,
                    Created = DateTime.UtcNow
                });
            });

            await _bookingRepository.Create(booking);
            await _bookingRepository.Create(bookingSeats);

            await _publishEndpoint.Publish(new BookingCreated(booking.Id, request.UserId, booking.TotalPrice));

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
