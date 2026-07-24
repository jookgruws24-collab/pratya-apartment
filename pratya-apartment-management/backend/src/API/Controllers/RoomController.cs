using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;

    public RoomController(
        IRoomRepository roomRepository
    )
    {
        _roomRepository = roomRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Room>>> GetAll()
    {
        var rooms = await _roomRepository.GetAllAsync();

        return Ok(rooms);
    }

    [HttpPost]
    public async Task<ActionResult> Create(
    CreateRoomDto dto
)
    {
        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomNumber = dto.RoomNumber,
            Floor = dto.Floor,
            ImageUrl = dto.ImageUrl,
            RoomStatusId = dto.RoomStatusId
        };

        await _roomRepository.AddAsync(room);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(
    Guid id
)
    {
        await _roomRepository.DeleteAsync(id);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(
    Guid id,
    UpdateRoomDto dto
)
    {
        var room = new Room
        {
            Id = id,
            RoomNumber = dto.RoomNumber,
            Floor = dto.Floor,
            ImageUrl = dto.ImageUrl,
            RoomStatusId = dto.RoomStatusId
        };

        await _roomRepository.UpdateAsync(room);

        return Ok();
    }
}
