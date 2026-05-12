using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateRoomDto
{
    [Required]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, 100)]
    public int Floor { get; set; }

    [Required]
    public int RoomStatusId { get; set; }

    public string? ImageUrl { get; set; }
}