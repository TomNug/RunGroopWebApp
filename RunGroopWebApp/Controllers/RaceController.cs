using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;

        // Dependency injection
        public RaceController(IRaceRepository raceRepository)
        {
            _raceRepository = raceRepository
;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAllAsync();
            return View(races);
        }

        // Don't need HttpGet
        //  Already laid on in MapControllerRoute
        //  "Conventional route"
        public async Task<IActionResult> Detail(int id)
        {
            // Entity doesn't explore JOINs by default
            //  Include forces it to explore the JOINs
            //  Finds the Addressnreferred to in the Club
            Race race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }


    }
}
