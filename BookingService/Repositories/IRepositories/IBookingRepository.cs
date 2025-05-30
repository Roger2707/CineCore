﻿using P4.BookingService.DTOs;
using P4.BookingService.Models;

namespace P4.BookingService.Repositories.IRepositories
{
    public interface IBookingRepository
    {
        Task<List<BookingDTO>> GetBookings();
        Task<BookingDTO> GetBooking(Guid bookingId);
        Task Create(Booking booking);
        Task Create(List<BookingSeat> bookingSeats);
        Task Delete(Guid bookingId);
        Task SaveChangeAsync();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task PublishMessageAsync<T>(T message) where T : class;
    }
}
