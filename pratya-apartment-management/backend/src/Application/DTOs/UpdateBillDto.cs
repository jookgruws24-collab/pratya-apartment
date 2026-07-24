using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class UpdateBillDto
{
    public decimal RentAmount { get; set; }

    public decimal WaterAmount { get; set; }

    public decimal ElectricAmount { get; set; }

    public decimal CommonFeeAmount { get; set; }

    public decimal LateFeeAmount { get; set; }

    [Required]
    public int BillStatusId { get; set; }

    public DateTime BillingMonth { get; set; }
}
