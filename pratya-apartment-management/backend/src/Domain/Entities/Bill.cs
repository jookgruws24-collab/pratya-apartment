namespace Domain.Entities;

public class Bill
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Room? Room { get; set; }

    public Guid TenantId { get; set; }

    public Tenant? Tenant { get; set; }

    public decimal RentAmount { get; set; }

    public decimal WaterAmount { get; set; }

    public decimal ElectricAmount { get; set; }

    public decimal CommonFeeAmount { get; set; }

    public decimal LateFeeAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public int BillStatusId { get; set; }

    public BillStatus? BillStatus { get; set; }

    public DateTime BillingMonth { get; set; }

    public DateTime CreatedAt { get; set; }
}