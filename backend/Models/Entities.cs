namespace ShippingCompany.Api.Models;

public abstract class EntityBase
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class User : EntityBase
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.Viewer;
    public bool IsActive { get; set; } = true;
}

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Dispatcher = "Dispatcher";
    public const string Finance = "Finance";
    public const string Viewer = "Viewer";

    public static readonly string[] All = [Admin, Dispatcher, Finance, Viewer];
}

public sealed class Vessel : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string ImoNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal CapacityTons { get; set; }
    public string CurrentPort { get; set; } = string.Empty;
    public VesselStatus Status { get; set; } = VesselStatus.Available;
    public DateTime? LastMaintenanceDate { get; set; }
    public List<TransportOrder> Orders { get; set; } = [];
}

public sealed class ShippingRoute : EntityBase
{
    public string RouteCode { get; set; } = string.Empty;
    public string OriginPort { get; set; } = string.Empty;
    public string DestinationPort { get; set; } = string.Empty;
    public decimal DistanceNm { get; set; }
    public int EstimatedDays { get; set; }
    public bool IsActive { get; set; } = true;
    public List<TransportOrder> Orders { get; set; } = [];
}

public sealed class Customer : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CreditCode { get; set; } = string.Empty;
    public List<Cargo> Cargoes { get; set; } = [];
    public List<TransportOrder> Orders { get; set; } = [];
}

public sealed class Cargo : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal WeightTons { get; set; }
    public decimal VolumeCbm { get; set; }
    public bool Hazardous { get; set; }
    public CargoStatus Status { get; set; } = CargoStatus.Pending;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public List<TransportOrder> Orders { get; set; } = [];
}

public sealed class TransportOrder : EntityBase
{
    public string OrderNo { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int CargoId { get; set; }
    public Cargo? Cargo { get; set; }
    public int? VesselId { get; set; }
    public Vessel? Vessel { get; set; }
    public int ShippingRouteId { get; set; }
    public ShippingRoute? ShippingRoute { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public int Progress { get; set; }
    public DateTime? PlannedDeparture { get; set; }
    public DateTime? PlannedArrival { get; set; }
    public DateTime? ActualDeparture { get; set; }
    public DateTime? ActualArrival { get; set; }
    public decimal FreightAmount { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public FinanceSettlement? Settlement { get; set; }
}

public sealed class FinanceSettlement : EntityBase
{
    public string SettlementNo { get; set; } = string.Empty;
    public int TransportOrderId { get; set; }
    public TransportOrder? TransportOrder { get; set; }
    public decimal ReceivableAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public SettlementStatus Status { get; set; } = SettlementStatus.Pending;
    public DateTime SettlementDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public List<PaymentRecord> Payments { get; set; } = [];
}

public sealed class PaymentRecord : EntityBase
{
    public int FinanceSettlementId { get; set; }
    public FinanceSettlement? FinanceSettlement { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime PaymentTime { get; set; } = DateTime.UtcNow;
    public string TransactionNo { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public enum VesselStatus
{
    Available,
    InTransit,
    Maintenance,
    Inactive
}

public enum CargoStatus
{
    Pending,
    Booked,
    InTransit,
    Delivered
}

public enum OrderStatus
{
    Draft,
    Scheduled,
    InTransit,
    Arrived,
    Completed,
    Cancelled
}

public enum SettlementStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    Overdue,
    Cancelled
}
