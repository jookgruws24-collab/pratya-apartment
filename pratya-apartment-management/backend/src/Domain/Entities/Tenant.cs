namespace Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;
}