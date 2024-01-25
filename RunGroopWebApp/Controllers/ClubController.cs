using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection
        public ClubController(ApplicationDbContext context)
        {
            _context = context; 
        }

        public IActionResult Index()
        {
            // Immediate execution
            List<Club> clubs = _context.Clubs.ToList();
            return View(clubs);
        }

        // Don't need HttpGet
        //  Already laid on in MapControllerRoute
        //  "Conventional route"
        public IActionResult Detail(int id)
        {
            // Entity doesn't explore JOINs by default
            //  Include forces it to explore the JOINs
            //  Finds the Addressnreferred to in the Club
            Club club = _context.Clubs.Include(a => a.Address).FirstOrDefault(c => c.Id == id);
            return View(club);
        }
    }
}
