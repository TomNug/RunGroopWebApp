using RunGroopWebApp.Models;

namespace RunGroopWebApp.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Club> Clubs { get; set; }
        public string City { get; set; }
        public string County { get; set; }
    }
}
