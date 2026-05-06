using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Data;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Models;

namespace ShippingCompany.Api.Services;

public sealed class FinanceService
{
    private readonly ShippingDbContext _context;

    public FinanceService(ShippingDbContext context)
    {
        _context = context;
    }

    public Task<PagedResult<FinanceSettlementDto>> ListAsync(string? keyword, int page, int pageSize)
    {
        var query = BaseQuery();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.SettlementNo.Contains(keyword) ||
                x.TransportOrder!.OrderNo.Contains(keyword) ||
                x.TransportOrder!.Customer!.Name.Contains(keyword));
        }

        return query.OrderByDescending(x => x.Id)
            .Select(x => ToDto(x))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<FinanceSettlementDto> CreateForOrderAsync(int orderId, CreateSettlementRequest request)
    {
        var order = await _context.TransportOrders
            .Include(x => x.Settlement)
            .FirstOrDefaultAsync(x => x.Id == orderId)
            ?? throw new KeyNotFoundException("order not found");

        if (order.Settlement is not null)
        {
            throw new InvalidOperationException("settlement already exists for this order");
        }

        var settlement = new FinanceSettlement
        {
            SettlementNo = await CreateSettlementNoAsync(),
            TransportOrderId = order.Id,
            ReceivableAmount = request.ReceivableAmount ?? order.FreightAmount,
            TaxAmount = request.TaxAmount ?? Math.Round(order.FreightAmount * 0.06m, 2),
            PaidAmount = 0,
            Status = SettlementStatus.Pending,
            DueDate = request.DueDate
        };

        _context.FinanceSettlements.Add(settlement);
        await _context.SaveChangesAsync();
        return await GetAsync(settlement.Id);
    }

    public async Task<FinanceSettlementDto> AddPaymentAsync(int settlementId, PaymentRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new InvalidOperationException("payment amount must be greater than zero");
        }

        var settlement = await _context.FinanceSettlements
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == settlementId)
            ?? throw new KeyNotFoundException("settlement not found");

        var payment = new PaymentRecord
        {
            FinanceSettlementId = settlement.Id,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod.Trim(),
            PaymentTime = request.PaymentTime ?? DateTime.UtcNow,
            TransactionNo = request.TransactionNo?.Trim() ?? string.Empty,
            Notes = request.Notes?.Trim() ?? string.Empty
        };

        settlement.Payments.Add(payment);
        settlement.PaidAmount += request.Amount;
        settlement.Status = settlement.PaidAmount >= settlement.ReceivableAmount
            ? SettlementStatus.Paid
            : SettlementStatus.PartiallyPaid;

        await _context.SaveChangesAsync();
        return await GetAsync(settlement.Id);
    }

    private async Task<FinanceSettlementDto> GetAsync(int id)
    {
        var settlement = await BaseQuery().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("settlement not found");
        return ToDto(settlement);
    }

    private IQueryable<FinanceSettlement> BaseQuery()
    {
        return _context.FinanceSettlements
            .AsNoTracking()
            .Include(x => x.TransportOrder)
            .ThenInclude(x => x!.Customer)
            .Include(x => x.Payments);
    }

    private async Task<string> CreateSettlementNoAsync()
    {
        var prefix = $"FS{DateTime.UtcNow:yyyyMMdd}";
        var count = await _context.FinanceSettlements.CountAsync(x => x.SettlementNo.StartsWith(prefix));
        return $"{prefix}{count + 1:0000}";
    }

    private static FinanceSettlementDto ToDto(FinanceSettlement settlement)
    {
        return new FinanceSettlementDto(
            settlement.Id,
            settlement.SettlementNo,
            settlement.TransportOrderId,
            settlement.TransportOrder?.OrderNo ?? string.Empty,
            settlement.TransportOrder?.Customer?.Name ?? string.Empty,
            settlement.ReceivableAmount,
            settlement.PaidAmount,
            settlement.TaxAmount,
            settlement.Status,
            settlement.SettlementDate,
            settlement.DueDate,
            settlement.Payments
                .OrderByDescending(x => x.PaymentTime)
                .Select(x => new PaymentRecordDto(
                    x.Id,
                    x.Amount,
                    x.PaymentMethod,
                    x.PaymentTime,
                    x.TransactionNo,
                    x.Notes))
                .ToList());
    }
}
