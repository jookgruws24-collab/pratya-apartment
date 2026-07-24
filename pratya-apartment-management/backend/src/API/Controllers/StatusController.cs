using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// รายการสถานะ (ห้อง/บิล) เอาไว้ทำ dropdown ในหน้าเว็บ
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatusController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StatusController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("rooms")]
    public async Task<IActionResult> RoomStatuses()
    {
        return Ok(await _context.RoomStatuses.ToListAsync());
    }

    [HttpGet("bills")]
    public async Task<IActionResult> BillStatuses()
    {
        return Ok(await _context.BillStatuses.ToListAsync());
    }
}
