using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Services;

namespace ShippingCompany.Api.Controllers;

[ApiController]
[Route("api/system")]
[Authorize(Roles = "Admin")]
public sealed class SystemController : ControllerBase
{
    private readonly BackupService _backupService;

    public SystemController(BackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpGet("backups")]
    public ActionResult<ApiResponse<IReadOnlyList<BackupDto>>> ListBackups()
    {
        var result = _backupService.ListBackups();
        return Ok(ApiResponse<IReadOnlyList<BackupDto>>.Ok(result));
    }

    [HttpPost("backups")]
    public async Task<ActionResult<ApiResponse<BackupDto>>> CreateBackup()
    {
        var result = await _backupService.CreateBackupAsync();
        return Ok(ApiResponse<BackupDto>.Ok(result));
    }
}
