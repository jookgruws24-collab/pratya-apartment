using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IBillRepository _billRepository;
    private readonly IPdfService _pdfService;

    public ReportController(
        IBillRepository billRepository,
        IPdfService pdfService)
    {
        _billRepository = billRepository;
        _pdfService = pdfService;
    }

    // ดาวน์โหลดรายงานบิลทั้งหมดเป็น PDF
    [HttpGet("bills/pdf")]
    public async Task<IActionResult> BillsPdf()
    {
        var bills = await _billRepository.GetAllAsync();

        var pdfBytes = _pdfService.GenerateBillsReport(bills);

        return File(pdfBytes, "application/pdf", "bills-report.pdf");
    }
}
