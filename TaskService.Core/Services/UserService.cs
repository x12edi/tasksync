using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskService.Core.Models;
using TaskService.Core.UnitOfWork;
using BCrypt.Net;
using TaskService.Core.Services;
using Microsoft.Extensions.Configuration;

namespace TaskService.Core.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = GenerateJwtToken(user);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.CompleteAsync();

        return token;
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Users.GetAllAsync();
        var targetUser = user.FirstOrDefault(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
        if (targetUser == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var newToken = GenerateJwtToken(targetUser);
        targetUser.RefreshToken = GenerateRefreshToken();
        targetUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.CompleteAsync();

        return newToken;
    }

    public async System.Threading.Tasks.Task RegisterAsync(string username, string password, string role)
    {
        var existingUser = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (existingUser != null)
            throw new InvalidOperationException("Username already exists");

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}