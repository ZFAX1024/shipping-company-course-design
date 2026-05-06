using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Common;
using ShippingCompany.Api.Data;
using ShippingCompany.Api.Dtos;
using ShippingCompany.Api.Models;

namespace ShippingCompany.Api.Services;

public sealed class UserService
{
    private readonly ShippingDbContext _context;
    private readonly PasswordService _passwordService;

    public UserService(ShippingDbContext context, PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public Task<PagedResult<UserDto>> ListAsync(string? keyword, int page, int pageSize)
    {
        var query = _context.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.UserName.Contains(keyword) ||
                x.DisplayName.Contains(keyword) ||
                x.Role.Contains(keyword));
        }

        return query
            .OrderBy(x => x.Id)
            .Select(x => ToDto(x))
            .ToPagedResultAsync(page, pageSize);
    }

    public async Task<UserDto> GetAsync(int id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("user not found");
        return ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        ValidateRole(request.Role);

        if (await _context.Users.AnyAsync(x => x.UserName == request.UserName))
        {
            throw new InvalidOperationException("username already exists");
        }

        var user = new User
        {
            UserName = request.UserName.Trim(),
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = request.Role,
            IsActive = request.IsActive
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserRequest request)
    {
        ValidateRole(request.Role);

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("user not found");

        user.DisplayName = request.DisplayName.Trim();
        user.Role = request.Role;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _passwordService.HashPassword(request.Password);
        }

        await _context.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("user not found");

        user.IsActive = false;
        await _context.SaveChangesAsync();
    }

    private static void ValidateRole(string role)
    {
        if (!UserRoles.All.Contains(role))
        {
            throw new InvalidOperationException("invalid role");
        }
    }

    private static UserDto ToDto(User user)
    {
        return new UserDto(
            user.Id,
            user.UserName,
            user.DisplayName,
            user.Role,
            user.IsActive,
            user.CreatedAt);
    }
}
