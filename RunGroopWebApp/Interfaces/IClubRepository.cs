using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IClubRepository
    {
        Task<IEnumerable<Club>> GetAllAsync();
        Task<Club> GetByIdAsync(int id);
        Task<Club> GetByIdNoTrackingAsync(int id);
        Task<IEnumerable<Club>> GetAllClubsByCityAsync(string city);

        Task<IEnumerable<Club>> GetNClubsByCityExludingIdAsync(string city, int n, int id);

        bool Add(Club club);
        bool Update(Club club);
        bool Delete(Club club);
        bool Save();
    }
}
