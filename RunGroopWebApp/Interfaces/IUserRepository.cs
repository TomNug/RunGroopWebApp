using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();

        Task<IEnumerable<AppUser>> GetAllUsersWithAddressAsync();
        Task<AppUser> GetUserByIdAsync(string id);

        Task<AppUser> GetUserByIdWithAddressAsync(string id);

        bool Add(AppUser user);
        bool Update(AppUser user);
        bool Delete(AppUser user);
        bool Save();
    }
}
