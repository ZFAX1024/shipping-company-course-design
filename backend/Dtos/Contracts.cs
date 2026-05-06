using System.ComponentModel.DataAnnotations;
using ShippingCompany.Api.Models;

namespace ShippingCompany.Api.Dtos;

public sealed record LoginRequest(
    [property: Required, MaxLength(50)] string UserName,
    [property: Required, MinLength(6), MaxLength(100)] string Password);

public sealed record LoginResponse(string Token, UserProfileDto User);

public sealed record UserProfileDto(int Id, string UserName, string DisplayName, string Role);

public sealed record UserDto(
    int Id,
    string UserName,
    string DisplayName,
    string Role,
    bool IsActive,
    DateTime CreatedAt);

public sealed record CreateUserRequest(
    [property: Required, MaxLength(50)] string UserName,
    [property: Required, MaxLength(80)] string DisplayName,
    [property: Required, MinLength(6), MaxLength(100)] string Password,
    [property: Required, MaxLength(32)] string Role,
    bool IsActive = true);

public sealed record UpdateUserRequest(
    [property: Required, MaxLength(80)] string DisplayName,
    [property: MinLength(6), MaxLength(100)] string? Password,
    [property: Required, MaxLength(32)] string Role,
    bool IsActive);

public sealed record VesselRequest(
    [property: Required, MaxLength(100)] string Name,
    [property: Required, MaxLength(30)] string ImoNumber,
    [property: Required, MaxLength(50)] string Type,
    [property: Range(0.01, double.MaxValue)] decimal CapacityTons,
    [property: Required, MaxLength(100)] string CurrentPort,
    VesselStatus Status,
    DateTime? LastMaintenanceDate);

public sealed record VesselDto(
    int Id,
    string Name,
    string ImoNumber,
    string Type,
    decimal CapacityTons,
    string CurrentPort,
    VesselStatus Status,
    DateTime? LastMaintenanceDate);

public sealed record ShippingRouteRequest(
    [property: Required, MaxLength(50)] string RouteCode,
    [property: Required, MaxLength(100)] string OriginPort,
    [property: Required, MaxLength(100)] string DestinationPort,
    [property: Range(0.01, double.MaxValue)] decimal DistanceNm,
    [property: Range(1, 365)] int EstimatedDays,
    bool IsActive);

public sealed record ShippingRouteDto(
    int Id,
    string RouteCode,
    string OriginPort,
    string DestinationPort,
    decimal DistanceNm,
    int EstimatedDays,
    bool IsActive);

public sealed record CustomerRequest(
    [property: Required, MaxLength(120)] string Name,
    [property: Required, MaxLength(80)] string ContactName,
    [property: Required, MaxLength(40)] string Phone,
    [property: EmailAddress, MaxLength(120)] string Email,
    [property: MaxLength(240)] string Address,
    [property: MaxLength(80)] string CreditCode);

public sealed record CustomerDto(
    int Id,
    string Name,
    string ContactName,
    string Phone,
    string Email,
    string Address,
    string CreditCode);

public sealed record CargoRequest(
    [property: Required, MaxLength(120)] string Name,
    [property: Required, MaxLength(80)] string Category,
    [property: Range(0.01, double.MaxValue)] decimal WeightTons,
    [property: Range(0.01, double.MaxValue)] decimal VolumeCbm,
    bool Hazardous,
    CargoStatus Status,
    [property: Range(1, int.MaxValue)] int CustomerId);

public sealed record CargoDto(
    int Id,
    string Name,
    string Category,
    decimal WeightTons,
    decimal VolumeCbm,
    bool Hazardous,
    CargoStatus Status,
    int CustomerId,
    string CustomerName);

public sealed record TransportOrderRequest(
    [property: Range(1, int.MaxValue)] int CustomerId,
    [property: Range(1, int.MaxValue)] int CargoId,
    [property: Range(1, int.MaxValue)] int ShippingRouteId,
    [property: Range(0.01, double.MaxValue)] decimal FreightAmount,
    DateTime? PlannedDeparture,
    DateTime? PlannedArrival,
    [property: MaxLength(500)] string? Remarks);

public sealed record DispatchOrderRequest(
    [property: Range(1, int.MaxValue)] int VesselId,
    DateTime? PlannedDeparture,
    DateTime? PlannedArrival);

public sealed record UpdateOrderStatusRequest(OrderStatus Status);

public sealed record UpdateOrderProgressRequest([property: Range(0, 100)] int Progress);

public sealed record TransportOrderDto(
    int Id,
    string OrderNo,
    int CustomerId,
    string CustomerName,
    int CargoId,
    string CargoName,
    int? VesselId,
    string? VesselName,
    int ShippingRouteId,
    string RouteName,
    OrderStatus Status,
    int Progress,
    DateTime? PlannedDeparture,
    DateTime? PlannedArrival,
    DateTime? ActualDeparture,
    DateTime? ActualArrival,
    decimal FreightAmount,
    string Remarks);

public sealed record CreateSettlementRequest(
    [property: Range(0.01, double.MaxValue)] decimal? ReceivableAmount,
    [property: Range(0, double.MaxValue)] decimal? TaxAmount,
    DateTime? DueDate);

public sealed record PaymentRequest(
    [property: Range(0.01, double.MaxValue)] decimal Amount,
    [property: Required, MaxLength(50)] string PaymentMethod,
    DateTime? PaymentTime,
    [property: MaxLength(80)] string? TransactionNo,
    [property: MaxLength(300)] string? Notes);

public sealed record PaymentRecordDto(
    int Id,
    decimal Amount,
    string PaymentMethod,
    DateTime PaymentTime,
    string TransactionNo,
    string Notes);

public sealed record FinanceSettlementDto(
    int Id,
    string SettlementNo,
    int TransportOrderId,
    string OrderNo,
    string CustomerName,
    decimal ReceivableAmount,
    decimal PaidAmount,
    decimal TaxAmount,
    SettlementStatus Status,
    DateTime SettlementDate,
    DateTime? DueDate,
    IReadOnlyList<PaymentRecordDto> Payments);

public sealed record DashboardSummaryDto(
    int VesselCount,
    int AvailableVesselCount,
    int CustomerCount,
    int ActiveRouteCount,
    int OrderCount,
    int InTransitOrderCount,
    int CompletedOrderCount,
    decimal PendingReceivableAmount);

public sealed record BackupDto(
    string FileName,
    string FullPath,
    long SizeBytes,
    DateTime CreatedAt);
