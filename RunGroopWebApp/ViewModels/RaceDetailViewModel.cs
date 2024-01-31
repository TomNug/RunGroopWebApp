using RunGroopWebApp.Models;

namespace RunGroopWebApp.ViewModels
{
    public class RaceDetailViewModel
    {
        public Race Race { get; set; }
        public IEnumerable<Race> RelatedRaces { get; set; }
    }
}
