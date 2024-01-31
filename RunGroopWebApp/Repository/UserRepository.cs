using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(AppUser user)
        {
            throw new NotImplementedException();
        }

        public bool Delete(AppUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }


        public async Task<IEnumerable<AppUser>> GetAllUsersWithAddressAsync()
        {
            return await _context.Users
                    .Include(u => u.Address)
                    .ToListAsync();
        }

        public async Task<List<AppUser>> GetNUsersByCityIncludingAddressExcludingIdAsync(int n, string city, string id)
        {
            return await _context.Users
                .Where(u => u.Address != null && u.Address.City.Contains(city) && u.Id != id)
                .Include(u => u.Address)
                .Take(n)
                .ToListAsync();
        }


        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByIdWithAddressAsync(string id)
        {
            return await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved == 0 ? true : false;
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
    }
}
