using CinemaService.DTOs;
using CinemaService.Models;
using CinemaService.Repositories.IRepositories;
using CinemaService.Services.IServices;

namespace CinemaService.Services
{
    public class TheaterService : ITheaterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TheaterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Retrieve
        public async Task<List<Theater>> GetAll(Guid cinemaId)
        {
            return await _unitOfWork.Theater.GetAll(cinemaId);
        }

        public async Task<Theater> GetbyId(Guid id)
        {
            return await _unitOfWork.Theater.GetbyId(id);
        }

        #endregion

        #region CRUD 

        public async Task Create(TheaterCreateDTO theaterCreateDTO)
        {
            var cinemaExisted = await _unitOfWork.Cinema.GetbyId(theaterCreateDTO.CinemaId) != null;
            if (!cinemaExisted) throw new Exception("Cinema not found");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var theater = new Theater
                {
                    Name = theaterCreateDTO.Name,
                    CinemaId = theaterCreateDTO.CinemaId,
                    TotalSeats = theaterCreateDTO.TotalSeats,
                };

                await _unitOfWork.Theater.Create(theater);
                await _unitOfWork.SaveChangesAsync();
                var seats = new List<Seat>();
                for (int i = 0; i < theaterCreateDTO.TotalSeats / 10; i++)
                {
                    char rowLetter = (char)('A' + i);

                    for (int col = 1; col <= 10; col++)
                    {
                       seats.Add(new Seat
                        {
                            RowName = rowLetter.ToString(),
                            SeatNumber = col,
                            TheaterId = theater.Id,
                        });
                    }
                }

                await _unitOfWork.Seat.Create(seats);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }          
        }

        public async Task Update(Guid id, TheaterUpdateDTO theaterUpdateDTO)
        {
            var theater = await _unitOfWork.Theater.GetbyId(id);
            if(theater == null) throw new Exception("Theater not found");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                theater.Name = theaterUpdateDTO.Name ?? theater.Name;
                theater.CinemaId = theaterUpdateDTO.CinemaId != Guid.Empty ? theaterUpdateDTO.CinemaId : theater.CinemaId;

                if (theaterUpdateDTO.TotalSeats != theater.TotalSeats / 10)
                {
                    theater.TotalSeats = theaterUpdateDTO.TotalSeats;
                    var seats = new List<Seat>();

                    // Update Seats
                    for (int i = 0; i < theaterUpdateDTO.TotalSeats / 10; i++)
                    {
                        char rowLetter = (char)('A' + i);

                        for (int col = 1; col <= 10; col++)
                        {
                            seats.Add(new Seat
                            {
                                RowName = rowLetter.ToString(),
                                SeatNumber = col,
                                TheaterId = theater.Id,
                            });
                        }
                    }
                    // AddRange Seats
                    await _unitOfWork.Seat.Create(seats);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(Guid id)
        {
            await _unitOfWork.Theater.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion
    }
}
