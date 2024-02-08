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
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Dependency injection
        public RaceController(IRaceRepository raceRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("Races")]
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
            // Detail race
            Race race = await _raceRepository.GetByIdAsync(id);

            // Related races
            IEnumerable<Race> relatedRaces = await _raceRepository.GetNRacesByCityExludingIdAsync(race.Address.City, 3, id);

            RaceDetailViewModel raceDetailViewModel = new RaceDetailViewModel()
            {
                Race = race,
                RelatedRaces = relatedRaces
            };

            return View(raceDetailViewModel);
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
                var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
                var result = await _photoService.AddPhotoAsync(raceViewModel.Image);

                var race = new Race
                {
                    Title = raceViewModel.Title,
                    Description = raceViewModel.Description,
                    ImageURL = result.Url.ToString(),
                    ImagePublicId = result.PublicId.ToString(),
                    RaceCategory = raceViewModel.RaceCategory,
                    AppUserId = currentUserId,
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
                Address = race.Address,
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

            var userRace = await _raceRepository.GetByIdNoTrackingAsync(id);

            if (userRace != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userRace.ImagePublicId);
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
                    ImageURL = photoResult.Url.ToString(),
                    ImagePublicId = photoResult.PublicId.ToString(),
                    AddressId = userRace.AddressId,
                    Address = raceViewModel.Address,
                    AppUserId = userRace.AppUserId,
                    RaceCategory = raceViewModel.RaceCategory
                };

                _raceRepository.Update(race);
                return RedirectToAction("Index");
            }
            else
            {
                return View(raceViewModel);
            }
        }

        public async Task<IActionResult> Delete (int id)
        {
            var raceDetails = await _raceRepository.GetByIdAsync(id);
            if (raceDetails == null)
            {
                return View("Error");
            }

            return View(raceDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteRace(int id)
        {
            var raceDetails = await _raceRepository.GetByIdAsync(id);
            if (raceDetails == null)
            {
                return View("Error");
            }

            // Try to delete the image
            try
            {
                await _photoService.DeletePhotoAsync(raceDetails.ImagePublicId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo");
                return View(raceDetails);
            }


            _raceRepository.Delete(raceDetails);
            return RedirectToAction("Index");
        }
    }
}
