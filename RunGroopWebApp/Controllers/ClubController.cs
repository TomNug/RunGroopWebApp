using Microsoft.AspNetCore.Mvc;
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
    }
}
