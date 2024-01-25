using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection
        public RaceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Race> races = _context.Races.ToList();
            return View(races);
        }

        // Don't need HttpGet
        //  Already laid on in MapControllerRoute
        //  "Conventional route"
        public IActionResult Detail(int id)
        {
            // Entity doesn't explore JOINs by default
            //  Include forces it to explore the JOINs
            //  Finds the Addressnreferred to in the Club
            Race race = _context.Races.Include(a => a.Address).FirstOrDefault(c => c.Id == id);
            return View(race);
        }


    }
}
