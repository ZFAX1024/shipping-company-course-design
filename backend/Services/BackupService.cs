using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Data;
using ShippingCompany.Api.Dtos;

namespace ShippingCompany.Api.Services;

public sealed class BackupService
{
    private readonly ShippingDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public BackupService(
        ShippingDbContext context,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        _environment = environment;
    }

    public IReadOnlyList<BackupDto> ListBackups()
    {
        var directory = GetBackupDirectory();
        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory.GetFiles(directory, "*.bak")
            .Select(path =>
            {
                var info = new FileInfo(path);
                return new BackupDto(info.Name, info.FullName, info.Length, info.CreationTimeUtc);
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<BackupDto> CreateBackupAsync()
    {
        var directory = GetBackupDirectory();
        Directory.CreateDirectory(directory);

        var connectionString = _context.Database.GetConnectionString()
            ?? throw new InvalidOperationException("connection string is missing");
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("database name is missing in connection string");
        }

        var fileName = $"{databaseName}_{DateTime.UtcNow:yyyyMMddHHmmss}.bak";
        var fullPath = Path.Combine(directory, fileName);
        var escapedDatabaseName = databaseName.Replace("]", "]]");

        await _context.Database.ExecuteSqlRawAsync(
            $"BACKUP DATABASE [{escapedDatabaseName}] TO DISK = {{0}} WITH INIT",
            fullPath);

        var info = new FileInfo(fullPath);
        return new BackupDto(info.Name, info.FullName, info.Length, info.CreationTimeUtc);
    }

    private string GetBackupDirectory()
    {
        var configured = _configuration["Backup:Directory"] ?? "backups";
        return Path.IsPathRooted(configured)
            ? configured
            : Path.GetFullPath(Path.Combine(_environment.ContentRootPath, configured));
    }
}
