using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateBillDto
{
    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public Guid TenantId { get; set; }

    public decimal RentAmount { get; set; }

    public decimal WaterAmount { get; set; }

    public decimal ElectricAmount { get; set; }

    public decimal CommonFeeAmount { get; set; }

    public decimal LateFeeAmount { get; set; }

    public int BillStatusId { get; set; }

    public DateTime BillingMonth { get; set; }
}