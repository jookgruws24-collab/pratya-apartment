using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    // สมัครสมาชิกใหม่
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterDto dto)
    {
        var existing = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existing is not null)
        {
            return BadRequest(new { message = "มีชื่อผู้ใช้นี้อยู่แล้ว" });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = _jwtTokenService.GenerateToken(user),
            Username = user.Username
        });
    }

    // เข้าสู่ระบบ
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);

        // ไม่บอกว่าผิดที่ username หรือ password เพื่อความปลอดภัย
        if (user is null ||
            !_passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(
                new { message = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง" });
        }

        return Ok(new AuthResponseDto
        {
            Token = _jwtTokenService.GenerateToken(user),
            Username = user.Username
        });
    }
}
