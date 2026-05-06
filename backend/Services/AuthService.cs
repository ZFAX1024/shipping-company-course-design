using Microsoft.EntityFrameworkCore;
using ShippingCompany.Api.Data;
using ShippingCompany.Api.Dtos;

namespace ShippingCompany.Api.Services;

public sealed class AuthService
{
    private readonly ShippingDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly TokenService _tokenService;

    public AuthService(
        ShippingDbContext context,
        PasswordService passwordService,
        TokenService tokenService)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName == request.UserName);

        if (user is null || !user.IsActive || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("invalid username or password");
        }

        return _tokenService.CreateLoginResponse(user);
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && x.IsActive);

        if (user is null)
        {
            throw new KeyNotFoundException("user not found");
        }

        return new UserProfileDto(user.Id, user.UserName, user.DisplayName, user.Role);
    }
}
