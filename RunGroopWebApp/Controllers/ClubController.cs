using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Data.Enum;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Dependency injection
        public ClubController(IClubRepository clubRepository, IPhotoService photoservice, IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _photoService = photoservice;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("Clubs")]
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

            // Related clubs
            IEnumerable<Club> relatedClubs = await _clubRepository.GetNClubsByCityExludingIdAsync(club.Address.City, 3, id);

            ClubDetailViewModel clubDetailViewModel = new ClubDetailViewModel()
            {
                Club = club,
                RelatedClubs = relatedClubs
            };



            return View(clubDetailViewModel);
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
                var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
                var result = await _photoService.AddPhotoAsync(clubViewModel.Image);

                var club = new Club
                {
                    Title = clubViewModel.Title,
                    Description = clubViewModel.Description,
                    ImageURL = result.Url.ToString(),
                    ImagePublicId = result.PublicId.ToString(),
                    ClubCategory = clubViewModel.ClubCategory,
                    AppUserId = currentUserId,
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
                Address = club.Address,
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

            var userClub = await _clubRepository.GetByIdNoTrackingAsync(id);

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
                    AddressId = userClub.AddressId,
                    Address = clubViewModel.Address,
                    AppUserId = userClub.AppUserId,
                    ClubCategory = clubViewModel.ClubCategory
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
