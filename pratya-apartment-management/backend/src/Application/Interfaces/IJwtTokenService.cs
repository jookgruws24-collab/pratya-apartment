using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtTokenService
{
    // สร้าง JWT token จากข้อมูลผู้ใช้
    string GenerateToken(User user);
}
