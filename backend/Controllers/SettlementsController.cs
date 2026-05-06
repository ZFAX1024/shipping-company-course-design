using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Services;

namespace ShippingCompany.Api.Controllers;

[ApiController]
[Route("api/settlements")]
[Authorize(Roles = "Admin,Finance")]
public sealed class SettlementsController : ControllerBase
{
    private readonly FinanceService _service;

    public SettlementsController(FinanceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<FinanceSettlementDto>>>> List(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.ListAsync(keyword, page, pageSize);
        return Ok(ApiResponse<PagedResult<FinanceSettlementDto>>.Ok(result));
    }

    [HttpPost("{id:int}/payments")]
    public async Task<ActionResult<ApiResponse<FinanceSettlementDto>>> AddPayment(int id, PaymentRequest request)
    {
        var result = await _service.AddPaymentAsync(id, request);
        return Ok(ApiResponse<FinanceSettlementDto>.Ok(result));
    }
}
