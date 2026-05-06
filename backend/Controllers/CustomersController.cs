using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Services;

namespace ShippingCompany.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public sealed class CustomersController : ControllerBase
{
    private readonly MasterDataService _service;

    public CustomersController(MasterDataService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerDto>>>> List(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.ListCustomersAsync(keyword, page, pageSize);
        return Ok(ApiResponse<PagedResult<CustomerDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> Get(int id)
    {
        var result = await _service.GetCustomerAsync(id);
        return Ok(ApiResponse<CustomerDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> Create(CustomerRequest request)
    {
        var result = await _service.CreateCustomerAsync(request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<CustomerDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> Update(int id, CustomerRequest request)
    {
        var result = await _service.UpdateCustomerAsync(id, request);
        return Ok(ApiResponse<CustomerDto>.Ok(result));
    }

    [Authorize(Roles = "Admin,Dispatcher")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        await _service.DeleteCustomerAsync(id);
        return Ok(ApiResponse.Ok());
    }
}
