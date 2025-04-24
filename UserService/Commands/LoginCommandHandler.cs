using BCrypt.Net;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserService.UnitOfWork;
using UserService.Models;

namespace UserService.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and password are required.");

            var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = GenerateJwtToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return token;
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
            if (key.Length < 32)
                throw new InvalidOperationException("JWT Key must be at least 32 characters long.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:TokenExpiryMinutes", 5)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}