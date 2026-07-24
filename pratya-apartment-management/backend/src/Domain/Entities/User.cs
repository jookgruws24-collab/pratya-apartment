namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    // เก็บเฉพาะค่า hash ของรหัสผ่าน ไม่เก็บรหัสผ่านจริง
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
