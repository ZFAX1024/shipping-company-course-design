using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Services;

namespace ShippingCompany.Api.Controllers;

[ApiController]
[Route("api/cargoes")]
[Authorize]
public sealed class CargoesController : ControllerBase
{
    private readonly MasterDataService _service;

    public CargoesController(MasterDataService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CargoDto>>>> List(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.ListCargoesAsync(keyword, page, pageSize);
        return Ok(ApiResponse<PagedResult<CargoDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CargoDto>>> Get(int id)
    {
        var result = await _service.GetCargoAsync(id);
        return Ok(ApiResponse<CargoDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CargoDto>>> Create(CargoRequest request)
    {
        var result = await _service.CreateCargoAsync(request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<CargoDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<CargoDto>>> Update(int id, CargoRequest request)
    {
        var result = await _service.UpdateCargoAsync(id, request);
        return Ok(ApiResponse<CargoDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        await _service.DeleteCargoAsync(id);
        return Ok(ApiResponse.Ok());
    }
}
