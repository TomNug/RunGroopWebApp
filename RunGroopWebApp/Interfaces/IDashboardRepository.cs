using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<Race>> GetAllUserRacesAsync();
        Task<List<Club>> GetAllUserClubsAsync();

        Task<AppUser> GetUserByIdAsync(string id);
        Task<AppUser> GetUserWithAddressByIdAsync(string id);
        Task<AppUser> GetUserWithAddressByIdNoTrackingAsync(string id);

        Task<AppUser> GetUserByIdNoTrackingAsync(string id);
        bool Update(AppUser user);
        bool Save();
    }
}
