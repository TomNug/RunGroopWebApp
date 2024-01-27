using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.Services;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;

        // Dependency injection
        public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
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


        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceViewModel.Image);

                var race = new Race
                {
                    Title = raceViewModel.Title,
                    Description = raceViewModel.Description,
                    Image = result.Url.ToString(),
                    Address = new Address
                    {
                        Street = raceViewModel.Address.Street,
                        City = raceViewModel.Address.City,
                        County = raceViewModel.Address.County,
                        Postcode = raceViewModel.Address.Postcode
                    }
                };

                _raceRepository.Add(race);
                return RedirectToAction("Index");
            }

            return View(raceViewModel);
        }

        // Find the existing race, populate the form with its data
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                return View("Error");
            }
            var raceViewModel = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                URL = race.Image,
                RaceCategory = race.RaceCategory
            };
            return View(raceViewModel);
        }

        // For the completed submitted form
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit race");
                return View("Edit", raceViewModel);
            }

            var userRace = await _raceRepository.GetByIdAsyncNoTracking(id);

            if (userRace != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userRace.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(raceViewModel);
                }
                var photoResult = await _photoService.AddPhotoAsync(raceViewModel.Image);

                var race = new Race
                {
                    Id = id,
                    Title = raceViewModel.Title,
                    Description = raceViewModel.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = raceViewModel.AddressId,
                    Address = raceViewModel.Address
                };

                _raceRepository.Update(race);
                return RedirectToAction("Index");
            }
            else
            {
                return View(raceViewModel);
            }
        }
    }
}
