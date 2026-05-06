using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Services;

namespace ShippingCompany.Api.Controllers;

[ApiController]
[Route("api/vessels")]
[Authorize]
public sealed class VesselsController : ControllerBase
{
    private readonly MasterDataService _service;

    public VesselsController(MasterDataService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<VesselDto>>>> List(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.ListVesselsAsync(keyword, page, pageSize);
        return Ok(ApiResponse<PagedResult<VesselDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<VesselDto>>> Get(int id)
    {
        var result = await _service.GetVesselAsync(id);
        return Ok(ApiResponse<VesselDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<VesselDto>>> Create(VesselRequest request)
    {
        var result = await _service.CreateVesselAsync(request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<VesselDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<VesselDto>>> Update(int id, VesselRequest request)
    {
        var result = await _service.UpdateVesselAsync(id, request);
        return Ok(ApiResponse<VesselDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        await _service.DeleteVesselAsync(id);
        return Ok(ApiResponse.Ok());
    }
}
