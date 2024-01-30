using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Club>> GetAllUserClubsAsync()
        {
            // ? is null-conditional. Checks if it's null before accessing
            // Returns null instead of null-reference
            var currentUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userClubs = _context.Clubs.Where(r => r.AppUser.Id == currentUser);
            return await userClubs.ToListAsync();

        }

        public async Task<List<Race>> GetAllUserRacesAsync()
        {
            // ? is null-conditional. Checks if it's null before accessing
            // Returns null instead of null-reference
            var currentUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userRaces = _context.Races.Where(r => r.AppUser.Id == currentUser);
            return await userRaces.ToListAsync();
        }

        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByIdNoTracking(string id)
        {
            return await _context.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserWithAddressById(string id)
        {
            return await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<AppUser> GetUserWithAddressByIdNoTracking(string id)
        {
            return await _context.Users
                .Include(u => u.Address)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(AppUser user)
        {
            _context.Users.Update(user);
            return Save();
        }
    }
}
