using RunGroopWebApp.Models;

namespace RunGroopWebApp.ViewModels
{
    public class ClubDetailViewModel
    {
        public Club Club { get; set; }
        public IEnumerable<Club> RelatedClubs { get; set; }
    }
}
