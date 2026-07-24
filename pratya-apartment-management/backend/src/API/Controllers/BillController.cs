using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillController : ControllerBase
{
    private readonly IBillRepository _billRepository;

    public BillController(
        IBillRepository billRepository
    )
    {
        _billRepository = billRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Bill>>> GetAll()
    {
        var bills = await _billRepository.GetAllAsync();

        return Ok(bills);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Bill>> GetById(Guid id)
    {
        var bill = await _billRepository.GetByIdAsync(id);

        if (bill is null)
        {
            return NotFound();
        }

        return Ok(bill);
    }

    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateBillDto dto
    )
    {
        var totalAmount =
            dto.RentAmount +
            dto.WaterAmount +
            dto.ElectricAmount +
            dto.CommonFeeAmount +
            dto.LateFeeAmount;

        var bill = new Bill
        {
            Id = Guid.NewGuid(),
            RoomId = dto.RoomId,
            TenantId = dto.TenantId,
            RentAmount = dto.RentAmount,
            WaterAmount = dto.WaterAmount,
            ElectricAmount = dto.ElectricAmount,
            CommonFeeAmount = dto.CommonFeeAmount,
            LateFeeAmount = dto.LateFeeAmount,
            TotalAmount = totalAmount,
            BillStatusId = dto.BillStatusId,
            BillingMonth = dto.BillingMonth,
            CreatedAt = DateTime.UtcNow
        };

        await _billRepository.AddAsync(bill);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateBillDto dto
    )
    {
        var bill = await _billRepository.GetByIdAsync(id);

        if (bill is null)
        {
            return NotFound();
        }

        bill.RentAmount = dto.RentAmount;
        bill.WaterAmount = dto.WaterAmount;
        bill.ElectricAmount = dto.ElectricAmount;
        bill.CommonFeeAmount = dto.CommonFeeAmount;
        bill.LateFeeAmount = dto.LateFeeAmount;
        bill.TotalAmount =
            dto.RentAmount +
            dto.WaterAmount +
            dto.ElectricAmount +
            dto.CommonFeeAmount +
            dto.LateFeeAmount;
        bill.BillStatusId = dto.BillStatusId;
        bill.BillingMonth = dto.BillingMonth;

        await _billRepository.UpdateAsync(bill);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _billRepository.DeleteAsync(id);

        return Ok();
    }
}
