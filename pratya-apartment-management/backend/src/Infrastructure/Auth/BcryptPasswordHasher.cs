using Application.Interfaces;

namespace Infrastructure.Auth;

// ใช้ไลบรารี BCrypt ซึ่งเป็นวิธี hash รหัสผ่านที่ปลอดภัยและนิยมใช้กัน
public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
