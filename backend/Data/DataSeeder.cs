using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Models;
using ShippingCompany.Api.Services;

namespace ShippingCompany.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ShippingDbContext context, PasswordService passwordService)
    {
        if (!await context.Users.AnyAsync())
        {
            context.Users.AddRange(
                CreateUser("admin", "System Administrator", UserRoles.Admin, passwordService),
                CreateUser("dispatcher", "Order Dispatcher", UserRoles.Dispatcher, passwordService),
                CreateUser("finance", "Finance Staff", UserRoles.Finance, passwordService),
                CreateUser("viewer", "Read Only User", UserRoles.Viewer, passwordService));
        }

        if (!await context.Vessels.AnyAsync())
        {
            context.Vessels.AddRange(
                new Vessel
                {
                    Name = "Ocean Pioneer",
                    ImoNumber = "IMO9000001",
                    Type = "Container Ship",
                    CapacityTons = 52000,
                    CurrentPort = "Shanghai",
                    Status = VesselStatus.Available,
                    LastMaintenanceDate = DateTime.UtcNow.AddMonths(-2)
                },
                new Vessel
                {
                    Name = "Pacific Trader",
                    ImoNumber = "IMO9000002",
                    Type = "Bulk Carrier",
                    CapacityTons = 68000,
                    CurrentPort = "Ningbo",
                    Status = VesselStatus.Maintenance,
                    LastMaintenanceDate = DateTime.UtcNow.AddDays(-5)
                });
        }

        if (!await context.Customers.AnyAsync())
        {
            context.Customers.AddRange(
                new Customer
                {
                    Name = "Blue Harbor Logistics",
                    ContactName = "Alice Chen",
                    Phone = "13800000001",
                    Email = "alice@example.com",
                    Address = "Shanghai Pudong Port Area",
                    CreditCode = "BHL20260001"
                },
                new Customer
                {
                    Name = "Eastern Steel Trading",
                    ContactName = "Bob Li",
                    Phone = "13800000002",
                    Email = "bob@example.com",
                    Address = "Ningbo Beilun District",
                    CreditCode = "EST20260002"
                });
        }

        if (!await context.ShippingRoutes.AnyAsync())
        {
            context.ShippingRoutes.AddRange(
                new ShippingRoute
                {
                    RouteCode = "SH-HK-001",
                    OriginPort = "Shanghai",
                    DestinationPort = "Hong Kong",
                    DistanceNm = 820,
                    EstimatedDays = 4,
                    IsActive = true
                },
                new ShippingRoute
                {
                    RouteCode = "NB-SG-001",
                    OriginPort = "Ningbo",
                    DestinationPort = "Singapore",
                    DistanceNm = 2200,
                    EstimatedDays = 8,
                    IsActive = true
                });
        }

        await context.SaveChangesAsync();

        if (!await context.Cargoes.AnyAsync())
        {
            var firstCustomer = await context.Customers.OrderBy(x => x.Id).FirstAsync();
            context.Cargoes.AddRange(
                new Cargo
                {
                    Name = "Consumer Electronics",
                    Category = "Container",
                    WeightTons = 18.5m,
                    VolumeCbm = 72,
                    Hazardous = false,
                    CustomerId = firstCustomer.Id,
                    Status = CargoStatus.Booked
                },
                new Cargo
                {
                    Name = "Steel Coils",
                    Category = "Bulk Cargo",
                    WeightTons = 1200,
                    VolumeCbm = 430,
                    Hazardous = false,
                    CustomerId = firstCustomer.Id,
                    Status = CargoStatus.Pending
                });
            await context.SaveChangesAsync();
        }

        if (!await context.TransportOrders.AnyAsync())
        {
            var customer = await context.Customers.OrderBy(x => x.Id).FirstAsync();
            var cargo = await context.Cargoes.OrderBy(x => x.Id).FirstAsync();
            var route = await context.ShippingRoutes.OrderBy(x => x.Id).FirstAsync();
            var vessel = await context.Vessels.OrderBy(x => x.Id).FirstAsync();

            context.TransportOrders.Add(new TransportOrder
            {
                OrderNo = $"SO{DateTime.UtcNow:yyyyMMdd}0001",
                CustomerId = customer.Id,
                CargoId = cargo.Id,
                ShippingRouteId = route.Id,
                VesselId = vessel.Id,
                Status = OrderStatus.Scheduled,
                Progress = 20,
                PlannedDeparture = DateTime.UtcNow.AddDays(1),
                PlannedArrival = DateTime.UtcNow.AddDays(5),
                FreightAmount = 86000,
                Remarks = "Seed order for course demonstration."
            });

            await context.SaveChangesAsync();
        }
    }

    private static User CreateUser(
        string userName,
        string displayName,
        string role,
        PasswordService passwordService)
    {
        return new User
        {
            UserName = userName,
            DisplayName = displayName,
            Role = role,
            PasswordHash = passwordService.HashPassword("123456"),
            IsActive = true
        };
    }
}
