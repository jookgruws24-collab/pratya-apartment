using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateTenantDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string RoomNumber { get; set; } = string.Empty;
}