using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantController : ControllerBase
{
    private readonly ITenantRepository _tenantRepository;

    public TenantController(
        ITenantRepository tenantRepository
    )
    {
        _tenantRepository = tenantRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Tenant>>> GetAll()
    {
        var tenants = await _tenantRepository.GetAllAsync();

        return Ok(tenants);
    }

    [HttpPost]
    public async Task<ActionResult> Create(
        [FromBody] CreateTenantDto dto
    )
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            RoomNumber = dto.RoomNumber
        };

        await _tenantRepository.AddAsync(tenant);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateTenantDto dto
    )
    {
        var tenant = new Tenant
        {
            Id = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            RoomNumber = dto.RoomNumber
        };

        await _tenantRepository.UpdateAsync(tenant);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(
        Guid id
    )
    {
        await _tenantRepository.DeleteAsync(id);

        return Ok();
    }
}