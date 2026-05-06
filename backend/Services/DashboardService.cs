using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Data;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Models;

namespace ShippingCompany.Api.Services;

public sealed class DashboardService
{
    private readonly ShippingDbContext _context;

    public DashboardService(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var pendingReceivable = await _context.FinanceSettlements
            .Where(x => x.Status != SettlementStatus.Paid && x.Status != SettlementStatus.Cancelled)
            .Select(x => (decimal?)(x.ReceivableAmount - x.PaidAmount))
            .SumAsync() ?? 0m;

        return new DashboardSummaryDto(
            VesselCount: await _context.Vessels.CountAsync(),
            AvailableVesselCount: await _context.Vessels.CountAsync(x => x.Status == VesselStatus.Available),
            CustomerCount: await _context.Customers.CountAsync(),
            ActiveRouteCount: await _context.ShippingRoutes.CountAsync(x => x.IsActive),
            OrderCount: await _context.TransportOrders.CountAsync(),
            InTransitOrderCount: await _context.TransportOrders.CountAsync(x => x.Status == OrderStatus.InTransit),
            CompletedOrderCount: await _context.TransportOrders.CountAsync(x => x.Status == OrderStatus.Completed),
            PendingReceivableAmount: pendingReceivable);
    }
}
