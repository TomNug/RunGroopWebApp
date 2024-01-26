using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;

        // Dependency injection
        public ClubController(IClubRepository clubRepository, IPhotoService photoservice)
        {
            _clubRepository = clubRepository;
            _photoService = photoservice;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAllAsync();
            return View(clubs);
        }

        // Don't need HttpGet
        //  Already laid on in MapControllerRoute
        //  "Conventional route"
        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(clubViewModel.Image);

                var club = new Club
                {
                    Title = clubViewModel.Title,
                    Description = clubViewModel.Description,
                    Image = result.Url.ToString(),
                    Address = new Address
                    {
                        Street = clubViewModel.Address.Street,
                        City = clubViewModel.Address.City,
                        County = clubViewModel.Address.County,
                        Postcode = clubViewModel.Address.Postcode
                    }
                };

                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }

            return View(clubViewModel);
            
            
        }
    }
}
