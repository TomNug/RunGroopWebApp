using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<Race>> GetAllUserRacesAsync();
        Task<List<Club>> GetAllUserClubsAsync();
    }
}
