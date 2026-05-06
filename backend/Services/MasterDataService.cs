using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Data;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Models;

namespace ShippingCompany.Api.Services;

public sealed class MasterDataService
{
    private readonly ShippingDbContext _context;

    public MasterDataService(ShippingDbContext context)
    {
        _context = context;
    }

    public Task<PagedResult<VesselDto>> ListVesselsAsync(string? keyword, int page, int pageSize)
    {
        var query = _context.Vessels.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.Name.Contains(keyword) ||
                x.ImoNumber.Contains(keyword) ||
                x.Type.Contains(keyword) ||
                x.CurrentPort.Contains(keyword));
        }

        return query.OrderBy(x => x.Id)
            .Select(x => ToDto(x))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<VesselDto> GetVesselAsync(int id)
    {
        var vessel = await _context.Vessels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("vessel not found");
        return ToDto(vessel);
    }

    public async Task<VesselDto> CreateVesselAsync(VesselRequest request)
    {
        if (await _context.Vessels.AnyAsync(x => x.ImoNumber == request.ImoNumber))
        {
            throw new InvalidOperationException("IMO number already exists");
        }

        var vessel = new Vessel();
        Apply(vessel, request);
        _context.Vessels.Add(vessel);
        await _context.SaveChangesAsync();
        return ToDto(vessel);
    }

    public async Task<VesselDto> UpdateVesselAsync(int id, VesselRequest request)
    {
        var vessel = await _context.Vessels.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("vessel not found");

        if (await _context.Vessels.AnyAsync(x => x.Id != id && x.ImoNumber == request.ImoNumber))
        {
            throw new InvalidOperationException("IMO number already exists");
        }

        Apply(vessel, request);
        await _context.SaveChangesAsync();
        return ToDto(vessel);
    }

    public async Task DeleteVesselAsync(int id)
    {
        var vessel = await _context.Vessels.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("vessel not found");
        _context.Vessels.Remove(vessel);
        await _context.SaveChangesAsync();
    }

    public Task<PagedResult<ShippingRouteDto>> ListRoutesAsync(string? keyword, int page, int pageSize)
    {
        var query = _context.ShippingRoutes.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.RouteCode.Contains(keyword) ||
                x.OriginPort.Contains(keyword) ||
                x.DestinationPort.Contains(keyword));
        }

        return query.OrderBy(x => x.Id)
            .Select(x => ToDto(x))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<ShippingRouteDto> GetRouteAsync(int id)
    {
        var route = await _context.ShippingRoutes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("route not found");
        return ToDto(route);
    }

    public async Task<ShippingRouteDto> CreateRouteAsync(ShippingRouteRequest request)
    {
        if (await _context.ShippingRoutes.AnyAsync(x => x.RouteCode == request.RouteCode))
        {
            throw new InvalidOperationException("route code already exists");
        }

        var route = new ShippingRoute();
        Apply(route, request);
        _context.ShippingRoutes.Add(route);
        await _context.SaveChangesAsync();
        return ToDto(route);
    }

    public async Task<ShippingRouteDto> UpdateRouteAsync(int id, ShippingRouteRequest request)
    {
        var route = await _context.ShippingRoutes.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("route not found");

        if (await _context.ShippingRoutes.AnyAsync(x => x.Id != id && x.RouteCode == request.RouteCode))
        {
            throw new InvalidOperationException("route code already exists");
        }

        Apply(route, request);
        await _context.SaveChangesAsync();
        return ToDto(route);
    }

    public async Task DeleteRouteAsync(int id)
    {
        if (await _context.TransportOrders.AnyAsync(x => x.ShippingRouteId == id))
        {
            throw new InvalidOperationException("route is used by orders");
        }

        var route = await _context.ShippingRoutes.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("route not found");
        _context.ShippingRoutes.Remove(route);
        await _context.SaveChangesAsync();
    }

    public Task<PagedResult<CustomerDto>> ListCustomersAsync(string? keyword, int page, int pageSize)
    {
        var query = _context.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.Name.Contains(keyword) ||
                x.ContactName.Contains(keyword) ||
                x.Phone.Contains(keyword) ||
                x.CreditCode.Contains(keyword));
        }

        return query.OrderBy(x => x.Id)
            .Select(x => ToDto(x))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<CustomerDto> GetCustomerAsync(int id)
    {
        var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("customer not found");
        return ToDto(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerRequest request)
    {
        var customer = new Customer();
        Apply(customer, request);
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return ToDto(customer);
    }

    public async Task<CustomerDto> UpdateCustomerAsync(int id, CustomerRequest request)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("customer not found");
        Apply(customer, request);
        await _context.SaveChangesAsync();
        return ToDto(customer);
    }

    public async Task DeleteCustomerAsync(int id)
    {
        if (await _context.TransportOrders.AnyAsync(x => x.CustomerId == id) ||
            await _context.Cargoes.AnyAsync(x => x.CustomerId == id))
        {
            throw new InvalidOperationException("customer is used by cargoes or orders");
        }

        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("customer not found");
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }

    public Task<PagedResult<CargoDto>> ListCargoesAsync(string? keyword, int page, int pageSize)
    {
        var query = _context.Cargoes.AsNoTracking().Include(x => x.Customer).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.Name.Contains(keyword) ||
                x.Category.Contains(keyword) ||
                x.Customer!.Name.Contains(keyword));
        }

        return query.OrderBy(x => x.Id)
            .Select(x => ToDto(x))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<CargoDto> GetCargoAsync(int id)
    {
        var cargo = await _context.Cargoes
            .AsNoTracking()
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("cargo not found");
        return ToDto(cargo);
    }

    public async Task<CargoDto> CreateCargoAsync(CargoRequest request)
    {
        await EnsureCustomerExistsAsync(request.CustomerId);

        var cargo = new Cargo();
        Apply(cargo, request);
        _context.Cargoes.Add(cargo);
        await _context.SaveChangesAsync();

        cargo.Customer = await _context.Customers.FindAsync(cargo.CustomerId);
        return ToDto(cargo);
    }

    public async Task<CargoDto> UpdateCargoAsync(int id, CargoRequest request)
    {
        await EnsureCustomerExistsAsync(request.CustomerId);

        var cargo = await _context.Cargoes.Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("cargo not found");

        Apply(cargo, request);
        await _context.SaveChangesAsync();
        cargo.Customer = await _context.Customers.FindAsync(cargo.CustomerId);
        return ToDto(cargo);
    }

    public async Task DeleteCargoAsync(int id)
    {
        if (await _context.TransportOrders.AnyAsync(x => x.CargoId == id))
        {
            throw new InvalidOperationException("cargo is used by orders");
        }

        var cargo = await _context.Cargoes.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("cargo not found");
        _context.Cargoes.Remove(cargo);
        await _context.SaveChangesAsync();
    }

    private async Task EnsureCustomerExistsAsync(int customerId)
    {
        if (!await _context.Customers.AnyAsync(x => x.Id == customerId))
        {
            throw new KeyNotFoundException("customer not found");
        }
    }

    private static void Apply(Vessel vessel, VesselRequest request)
    {
        vessel.Name = request.Name.Trim();
        vessel.ImoNumber = request.ImoNumber.Trim();
        vessel.Type = request.Type.Trim();
        vessel.CapacityTons = request.CapacityTons;
        vessel.CurrentPort = request.CurrentPort.Trim();
        vessel.Status = request.Status;
        vessel.LastMaintenanceDate = request.LastMaintenanceDate;
    }

    private static void Apply(ShippingRoute route, ShippingRouteRequest request)
    {
        route.RouteCode = request.RouteCode.Trim();
        route.OriginPort = request.OriginPort.Trim();
        route.DestinationPort = request.DestinationPort.Trim();
        route.DistanceNm = request.DistanceNm;
        route.EstimatedDays = request.EstimatedDays;
        route.IsActive = request.IsActive;
    }

    private static void Apply(Customer customer, CustomerRequest request)
    {
        customer.Name = request.Name.Trim();
        customer.ContactName = request.ContactName.Trim();
        customer.Phone = request.Phone.Trim();
        customer.Email = request.Email.Trim();
        customer.Address = request.Address.Trim();
        customer.CreditCode = request.CreditCode.Trim();
    }

    private static void Apply(Cargo cargo, CargoRequest request)
    {
        cargo.Name = request.Name.Trim();
        cargo.Category = request.Category.Trim();
        cargo.WeightTons = request.WeightTons;
        cargo.VolumeCbm = request.VolumeCbm;
        cargo.Hazardous = request.Hazardous;
        cargo.Status = request.Status;
        cargo.CustomerId = request.CustomerId;
    }

    private static VesselDto ToDto(Vessel vessel)
    {
        return new VesselDto(
            vessel.Id,
            vessel.Name,
            vessel.ImoNumber,
            vessel.Type,
            vessel.CapacityTons,
            vessel.CurrentPort,
            vessel.Status,
            vessel.LastMaintenanceDate);
    }

    private static ShippingRouteDto ToDto(ShippingRoute route)
    {
        return new ShippingRouteDto(
            route.Id,
            route.RouteCode,
            route.OriginPort,
            route.DestinationPort,
            route.DistanceNm,
            route.EstimatedDays,
            route.IsActive);
    }

    private static CustomerDto ToDto(Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.ContactName,
            customer.Phone,
            customer.Email,
            customer.Address,
            customer.CreditCode);
    }

    private static CargoDto ToDto(Cargo cargo)
    {
        return new CargoDto(
            cargo.Id,
            cargo.Name,
            cargo.Category,
            cargo.WeightTons,
            cargo.VolumeCbm,
            cargo.Hazardous,
            cargo.Status,
            cargo.CustomerId,
            cargo.Customer?.Name ?? string.Empty);
    }
}
