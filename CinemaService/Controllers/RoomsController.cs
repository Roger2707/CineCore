using CinemaService.DTOs;
using CinemaService.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CinemaService.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("get-all-in-cine")]
        public async Task<IActionResult> GetAll(Guid cinemaId)
        {
            var rooms = await _roomService.GetAll(cinemaId);
            return Ok(rooms);
        }

        [HttpGet("get-room")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _roomService.GetRoom(id);
            if(room == null) return NotFound("Room not found");
            return Ok(room);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoomCreateDTO roomCreateDTO)
        {
            try
            {
                await _roomService.Create(roomCreateDTO);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoomUpdateDTO roomUpdateDTO)
        {
            try
            {
                await _roomService.UpdateRoom(id, roomUpdateDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _roomService.DeleteRoom(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
