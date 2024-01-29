using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;
        public DashboardController(IDashboardRepository dashboardRepository, 
            IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
        {
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
        }

        private void MapUserEdit(AppUser user, EditUserDashboardViewModel editViewModel,
            ImageUploadResult photoResult)
        {
            //user.Id = editViewModel.Id;
            user.Pace = editViewModel.Pace;
            user.Mileage = editViewModel.Mileage;
            user.ProfileImagePublicId = photoResult.PublicId;
            user.ProfileImageUrl = photoResult.Url.ToString();
            user.City = editViewModel.City;
            user.County = editViewModel.County;
        }

        // Repository calls are async, so whole method must be
        public async Task<IActionResult> Index()
        {
            var userRaces = await _dashboardRepository.GetAllUserRacesAsync();
            var userClubs = await _dashboardRepository.GetAllUserClubsAsync();

            var dashboardViewModel = new DashboardViewModel()
            {
                Races = userRaces,
                Clubs = userClubs
            };
            return View(dashboardViewModel);
        }

        public async Task<IActionResult> EditUserProfile()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();

            if (currentUserId == null)
            {
                return View("Error");
            }
            var user = await _dashboardRepository.GetUserById(currentUserId);

            if (user == null)
            {
                return View("Error");
            }

            var editUserDashboardViewModel = new EditUserDashboardViewModel()
            {
                Id = currentUserId,
                Pace = user.Pace,
                Mileage = user.Mileage,
                //ProfileImageUrl = user.ProfileImageUrl,
                City = user.City,
                County = user.County
            };

            return View(editUserDashboardViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditUserProfile", editViewModel);
            }

            
            var user = await _dashboardRepository.GetUserByIdNoTracking(editViewModel.Id);

            // No existing photo
            if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
            {
                // Add a new one
                var photoResult = await _photoService.AddPhotoAsync(editViewModel.Image);

                MapUserEdit(user, editViewModel, photoResult);

                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }

            // Existing photo, delete existing
            else
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImagePublicId);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(editViewModel);
                }

                // Add a new one
                var photoResult = await _photoService.AddPhotoAsync(editViewModel.Image);

                MapUserEdit(user, editViewModel, photoResult);

                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }
        }
    }
}
