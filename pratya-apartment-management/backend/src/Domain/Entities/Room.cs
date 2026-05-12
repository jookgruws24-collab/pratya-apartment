namespace Domain.Entities;

public class Room
{
    public Guid Id { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public int Floor { get; set; }

    public string? ImageUrl { get; set; }

    public int RoomStatusId { get; set; }

    public RoomStatus? RoomStatus { get; set; }
}