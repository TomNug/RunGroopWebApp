using RunGroopWebApp.Models;

namespace RunGroopWebApp.ViewModels
{
    public class DashboardViewModel
    {
        public List<Race> Races { get; set; }
        public List<Club> Clubs { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int? Pace { get; set; }
        public int? Mileage { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? Postcode { get; set; }
    }
}
