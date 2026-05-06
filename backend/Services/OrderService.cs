using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Data;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Models;

namespace ShippingCompany.Api.Services;

public sealed class OrderService
{
    private readonly ShippingDbContext _context;

    public OrderService(ShippingDbContext context)
    {
        _context = context;
    }

    public Task<PagedResult<TransportOrderDto>> ListAsync(string? keyword, int page, int pageSize)
    {
        var query = BaseQuery();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.OrderNo.Contains(keyword) ||
                x.Customer!.Name.Contains(keyword) ||
                x.Cargo!.Name.Contains(keyword) ||
                x.ShippingRoute!.OriginPort.Contains(keyword) ||
                x.ShippingRoute!.DestinationPort.Contains(keyword));
        }

        return query.OrderByDescending(x => x.Id)
            .Select(x => ToDto(x))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<TransportOrderDto> GetAsync(int id)
    {
        var order = await BaseQuery().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("order not found");
        return ToDto(order);
    }

    public async Task<TransportOrderDto> CreateAsync(TransportOrderRequest request)
    {
        await EnsureExistsAsync<Customer>(request.CustomerId, "customer not found");
        await EnsureExistsAsync<Cargo>(request.CargoId, "cargo not found");
        await EnsureExistsAsync<ShippingRoute>(request.ShippingRouteId, "route not found");

        var order = new TransportOrder
        {
            OrderNo = await CreateOrderNoAsync(),
            CustomerId = request.CustomerId,
            CargoId = request.CargoId,
            ShippingRouteId = request.ShippingRouteId,
            FreightAmount = request.FreightAmount,
            PlannedDeparture = request.PlannedDeparture,
            PlannedArrival = request.PlannedArrival,
            Remarks = request.Remarks?.Trim() ?? string.Empty,
            Status = OrderStatus.Draft,
            Progress = 0
        };

        var cargo = await _context.Cargoes.FirstAsync(x => x.Id == request.CargoId);
        cargo.Status = CargoStatus.Booked;

        _context.TransportOrders.Add(order);
        await _context.SaveChangesAsync();
        return await GetAsync(order.Id);
    }

    public async Task<TransportOrderDto> UpdateAsync(int id, TransportOrderRequest request)
    {
        await EnsureExistsAsync<Customer>(request.CustomerId, "customer not found");
        await EnsureExistsAsync<Cargo>(request.CargoId, "cargo not found");
        await EnsureExistsAsync<ShippingRoute>(request.ShippingRouteId, "route not found");

        var order = await _context.TransportOrders.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("order not found");

        order.CustomerId = request.CustomerId;
        order.CargoId = request.CargoId;
        order.ShippingRouteId = request.ShippingRouteId;
        order.FreightAmount = request.FreightAmount;
        order.PlannedDeparture = request.PlannedDeparture;
        order.PlannedArrival = request.PlannedArrival;
        order.Remarks = request.Remarks?.Trim() ?? string.Empty;

        await _context.SaveChangesAsync();
        return await GetAsync(order.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _context.TransportOrders
            .Include(x => x.Settlement)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("order not found");

        if (order.Settlement is not null)
        {
            throw new InvalidOperationException("order has settlement records");
        }

        _context.TransportOrders.Remove(order);
        await _context.SaveChangesAsync();
    }

    public async Task<TransportOrderDto> DispatchAsync(int id, DispatchOrderRequest request)
    {
        var order = await _context.TransportOrders
            .Include(x => x.ShippingRoute)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("order not found");

        var vessel = await _context.Vessels.FirstOrDefaultAsync(x => x.Id == request.VesselId)
            ?? throw new KeyNotFoundException("vessel not found");

        if (vessel.Status is VesselStatus.Inactive or VesselStatus.Maintenance)
        {
            throw new InvalidOperationException("vessel is not available for dispatch");
        }

        var plannedDeparture = request.PlannedDeparture ?? order.PlannedDeparture ?? DateTime.UtcNow;
        var plannedArrival = request.PlannedArrival
            ?? order.PlannedArrival
            ?? plannedDeparture.AddDays(order.ShippingRoute?.EstimatedDays ?? 1);

        order.VesselId = request.VesselId;
        order.PlannedDeparture = plannedDeparture;
        order.PlannedArrival = plannedArrival;
        order.Status = OrderStatus.Scheduled;
        order.Progress = Math.Max(order.Progress, 5);

        await _context.SaveChangesAsync();
        return await GetAsync(order.Id);
    }

    public async Task<TransportOrderDto> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.TransportOrders
            .Include(x => x.Cargo)
            .Include(x => x.Vessel)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("order not found");

        order.Status = status;

        if (status == OrderStatus.InTransit)
        {
            order.ActualDeparture ??= DateTime.UtcNow;
            order.Progress = Math.Max(order.Progress, 10);
            if (order.Cargo is not null)
            {
                order.Cargo.Status = CargoStatus.InTransit;
            }
            if (order.Vessel is not null)
            {
                order.Vessel.Status = VesselStatus.InTransit;
            }
        }
        else if (status == OrderStatus.Arrived)
        {
            order.Progress = Math.Max(order.Progress, 90);
        }
        else if (status == OrderStatus.Completed)
        {
            order.Progress = 100;
            order.ActualArrival ??= DateTime.UtcNow;
            if (order.Cargo is not null)
            {
                order.Cargo.Status = CargoStatus.Delivered;
            }
            if (order.Vessel is not null)
            {
                order.Vessel.Status = VesselStatus.Available;
            }
        }
        else if (status == OrderStatus.Cancelled && order.Vessel is not null)
        {
            order.Vessel.Status = VesselStatus.Available;
        }

        await _context.SaveChangesAsync();
        return await GetAsync(order.Id);
    }

    public async Task<TransportOrderDto> UpdateProgressAsync(int id, int progress)
    {
        if (progress is < 0 or > 100)
        {
            throw new InvalidOperationException("progress must be between 0 and 100");
        }

        var order = await _context.TransportOrders.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("order not found");

        order.Progress = progress;
        if (progress == 100)
        {
            order.Status = OrderStatus.Completed;
            order.ActualArrival ??= DateTime.UtcNow;
        }
        else if (progress > 0 && order.Status is OrderStatus.Draft or OrderStatus.Scheduled)
        {
            order.Status = OrderStatus.InTransit;
            order.ActualDeparture ??= DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return await GetAsync(order.Id);
    }

    private IQueryable<TransportOrder> BaseQuery()
    {
        return _context.TransportOrders
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Cargo)
            .Include(x => x.Vessel)
            .Include(x => x.ShippingRoute);
    }

    private async Task EnsureExistsAsync<TEntity>(int id, string message)
        where TEntity : EntityBase
    {
        if (!await _context.Set<TEntity>().AnyAsync(x => x.Id == id))
        {
            throw new KeyNotFoundException(message);
        }
    }

    private async Task<string> CreateOrderNoAsync()
    {
        var prefix = $"SO{DateTime.UtcNow:yyyyMMdd}";
        var count = await _context.TransportOrders.CountAsync(x => x.OrderNo.StartsWith(prefix));
        return $"{prefix}{count + 1:0000}";
    }

    private static TransportOrderDto ToDto(TransportOrder order)
    {
        var routeName = order.ShippingRoute is null
            ? string.Empty
            : $"{order.ShippingRoute.OriginPort} - {order.ShippingRoute.DestinationPort}";

        return new TransportOrderDto(
            order.Id,
            order.OrderNo,
            order.CustomerId,
            order.Customer?.Name ?? string.Empty,
            order.CargoId,
            order.Cargo?.Name ?? string.Empty,
            order.VesselId,
            order.Vessel?.Name,
            order.ShippingRouteId,
            routeName,
            order.Status,
            order.Progress,
            order.PlannedDeparture,
            order.PlannedArrival,
            order.ActualDeparture,
            order.ActualArrival,
            order.FreightAmount,
            order.Remarks);
    }
}
