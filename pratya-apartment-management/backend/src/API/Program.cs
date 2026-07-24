using System.Text;
using Application.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Pdf;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1) Controllers + Swagger (หน้าเว็บสำหรับทดลองเรียก API)
// ---------------------------------------------------------------------------
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pratya Apartment API",
        Version = "v1"
    });

    // เพิ่มช่องใส่ JWT token ในหน้า Swagger (ปุ่ม Authorize)
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "ใส่เฉพาะ token ไม่ต้องพิมพ์คำว่า Bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    options.AddSecurityDefinition("Bearer", scheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { scheme, Array.Empty<string>() }
    });
});

// ---------------------------------------------------------------------------
// 2) Database (PostgreSQL ผ่าน EF Core)
// ---------------------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

// ---------------------------------------------------------------------------
// 3) อ่านค่าตั้งค่าจาก appsettings / environment variable
// ---------------------------------------------------------------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<StorageSettings>(
    builder.Configuration.GetSection("Storage"));

// ---------------------------------------------------------------------------
// 4) ลงทะเบียน Repository และ Service ต่าง ๆ (Dependency Injection)
// ---------------------------------------------------------------------------
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBillRepository, BillRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPdfService, PdfService>();

// เลือกที่เก็บไฟล์ตามค่า Storage:Provider (Local = disk, Azure = Blob)
var storageProvider =
    builder.Configuration.GetValue<string>("Storage:Provider") ?? "Local";
if (storageProvider.Equals("Azure", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IFileStorageService, AzureBlobStorageService>();
}
else
{
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
}

// ---------------------------------------------------------------------------
// 5) JWT Authentication (ตรวจสอบ token ในทุก request ที่ต้องล็อกอิน)
// ---------------------------------------------------------------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecret = jwtSection.GetValue<string>("Secret") ?? string.Empty;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.GetValue<string>("Issuer"),
            ValidAudience = jwtSection.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// 6) CORS (อนุญาตให้ frontend เรียก API ได้)
// ---------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// 7) อัปเดตฐานข้อมูลให้เป็นเวอร์ชันล่าสุดอัตโนมัติ (สะดวกตอนรันด้วย Docker)
// ---------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// ---------------------------------------------------------------------------
// 8) Middleware pipeline (ลำดับสำคัญ)
// ---------------------------------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();

// เปิดให้เข้าถึงไฟล์ที่อัปโหลดไว้ในโฟลเดอร์ wwwroot/uploads
// สร้างโฟลเดอร์ให้แน่ใจว่ามีอยู่ แล้วชี้ static files ไปที่นั่นแบบชัดเจน
var webRootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(Path.Combine(webRootPath, "uploads"));
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webRootPath)
});

app.UseCors("AllowFrontend");

app.UseAuthentication(); // ตรวจ token ก่อน
app.UseAuthorization();  // แล้วค่อยเช็คสิทธิ์

app.MapControllers();

app.Run();
