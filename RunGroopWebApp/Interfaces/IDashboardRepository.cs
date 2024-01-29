using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<Race>> GetAllUserRacesAsync();
        Task<List<Club>> GetAllUserClubsAsync();

        Task<AppUser> GetUserById(string id);

        Task<AppUser> GetUserByIdNoTracking(string id);
        bool Update(AppUser user);
        bool Save();
    }
}
