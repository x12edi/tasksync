using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserService.UnitOfWork;
using UserService.Models;
using UserService.Queries;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }

        public IQueryable<User> GetAll()
        {
            return _context.Users.AsQueryable();
        }

        //public async Task<(List<UserDto> Users, int Total)> GetUsersAsync(int page, int pageSize, string? sortBy, string? sortOrder)
        //{
        //    var query = _context.Users.ToListAsync();

        //    if (!string.IsNullOrEmpty(sortBy))
        //    {
        //        query = sortBy.ToLower() switch
        //        {
        //            "username" => sortOrder == "desc" ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
        //            "role" => sortOrder == "desc" ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
        //            _ => query
        //        };
        //    }

        //    var total = await query.CountAsync();
        //    var users = await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(u => new UserDto
        //        {
        //            Id = u.Id,
        //            Username = u.Username,
        //            Role = u.Role
        //        })
        //        .ToListAsync();

        //    return (users, total);
        //}
    }
}