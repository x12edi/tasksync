using BCrypt.Net;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UserService.UnitOfWork;
using UserService.Models;

namespace UserService.Commands
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, User>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and password are required.");

            if (request.Password.Length < 8 || !request.Password.Any(char.IsUpper) || !request.Password.Any(char.IsDigit))
                throw new ArgumentException("Password must be at least 8 characters long with an uppercase letter and a digit.");

            var existingUser = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists");

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user;
        }
    }
}