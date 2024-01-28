using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;
using System.Reflection.Metadata.Ecma335;

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
                    ImageURL = result.Url.ToString(),
                    ImagePublicId = result.PublicId.ToString(),
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

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return View("Error");
            }
            var clubViewModel = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                Address = club.Address,
                URL = club.ImageURL,
                ClubCategory = club.ClubCategory
            };
            return View(clubViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", clubViewModel);
            }

            var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);

            if (userClub != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.ImagePublicId);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(clubViewModel);
                }
                var photoResult = await _photoService.AddPhotoAsync(clubViewModel.Image);

                var club = new Club
                {
                    Id = id,
                    Title = clubViewModel.Title,
                    Description = clubViewModel.Description,
                    ImageURL = photoResult.Url.ToString(),
                    ImagePublicId = photoResult.PublicId.ToString(),
                    AddressId = clubViewModel.AddressId,
                    Address = clubViewModel.Address
                };

                _clubRepository.Update(club);
                return RedirectToAction("Index");
            }
            else
            {
                return View(clubViewModel);
            }
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            var clubDetails = await _clubRepository.GetByIdAsync(id);
            if (clubDetails == null)
            {
                return View("Error");
            }
            return View(clubDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var clubDetails = await _clubRepository.GetByIdAsync(id);
            if (clubDetails == null)
            {
                return View("Error");
            }

            // Try to delete the image
            try
            {
                await _photoService.DeletePhotoAsync(clubDetails.ImagePublicId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo");
                return View(clubDetails);
            }

            _clubRepository.Delete(clubDetails);

            return RedirectToAction("Index");
        }
    }
}
