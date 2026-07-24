namespace Infrastructure.Auth;

// ค่าตั้งค่าของ JWT ที่อ่านมาจาก appsettings / environment variable
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    // อายุของ token (นาที)
    public int ExpiryMinutes { get; set; } = 120;
}
