using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Services;

namespace ShippingCompany.Api.Controllers;

[ApiController]
[Route("api/routes")]
[Authorize]
public sealed class RoutesController : ControllerBase
{
    private readonly MasterDataService _service;

    public RoutesController(MasterDataService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ShippingRouteDto>>>> List(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.ListRoutesAsync(keyword, page, pageSize);
        return Ok(ApiResponse<PagedResult<ShippingRouteDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ShippingRouteDto>>> Get(int id)
    {
        var result = await _service.GetRouteAsync(id);
        return Ok(ApiResponse<ShippingRouteDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ShippingRouteDto>>> Create(ShippingRouteRequest request)
    {
        var result = await _service.CreateRouteAsync(request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<ShippingRouteDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ShippingRouteDto>>> Update(int id, ShippingRouteRequest request)
    {
        var result = await _service.UpdateRouteAsync(id, request);
        return Ok(ApiResponse<ShippingRouteDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        await _service.DeleteRouteAsync(id);
        return Ok(ApiResponse.Ok());
    }
}
