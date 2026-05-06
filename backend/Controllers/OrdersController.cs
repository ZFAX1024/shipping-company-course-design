using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Services;

namespace ShippingCompany.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrdersController : ControllerBase
{
    private readonly OrderService _service;
    private readonly FinanceService _financeService;

    public OrdersController(OrderService service, FinanceService financeService)
    {
        _service = service;
        _financeService = financeService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TransportOrderDto>>>> List(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.ListAsync(keyword, page, pageSize);
        return Ok(ApiResponse<PagedResult<TransportOrderDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<TransportOrderDto>>> Get(int id)
    {
        var result = await _service.GetAsync(id);
        return Ok(ApiResponse<TransportOrderDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TransportOrderDto>>> Create(TransportOrderRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<TransportOrderDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<TransportOrderDto>>> Update(int id, TransportOrderRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<TransportOrderDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse.Ok());
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost("{id:int}/dispatch")]
    public async Task<ActionResult<ApiResponse<TransportOrderDto>>> Dispatch(int id, DispatchOrderRequest request)
    {
        var result = await _service.DispatchAsync(id, request);
        return Ok(ApiResponse<TransportOrderDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<TransportOrderDto>>> UpdateStatus(int id, UpdateOrderStatusRequest request)
    {
        var result = await _service.UpdateStatusAsync(id, request.Status);
        return Ok(ApiResponse<TransportOrderDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPatch("{id:int}/progress")]
    public async Task<ActionResult<ApiResponse<TransportOrderDto>>> UpdateProgress(int id, UpdateOrderProgressRequest request)
    {
        var result = await _service.UpdateProgressAsync(id, request.Progress);
        return Ok(ApiResponse<TransportOrderDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Finance")]
    [HttpPost("{id:int}/settlement")]
    public async Task<ActionResult<ApiResponse<FinanceSettlementDto>>> CreateSettlement(
        int id,
        CreateSettlementRequest request)
    {
        var result = await _financeService.CreateForOrderAsync(id, request);
        return Ok(ApiResponse<FinanceSettlementDto>.Ok(result));
    }
}
