using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
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

        private void MapUserOntoViewModel(AppUser user, DashboardViewModel dashboardViewModel)
        {
            dashboardViewModel.UserName = user.UserName;
            dashboardViewModel.ProfilePictureUrl = user.ProfileImageUrl;
            dashboardViewModel.Pace = user.Pace;
            dashboardViewModel.Mileage = user.Mileage;
            dashboardViewModel.Street = user.Address?.Street;
            dashboardViewModel.City = user.Address?.City;
            dashboardViewModel.County = user.Address?.County;
            dashboardViewModel.Postcode = user.Address?.Postcode;
        }

        // Repository calls are async, so whole method must be
        public async Task<IActionResult> Index()
        {
            var userRaces = await _dashboardRepository.GetAllUserRacesAsync();
            var userClubs = await _dashboardRepository.GetAllUserClubsAsync();

            var dashboardViewModel = new DashboardViewModel()
            {
                Races = userRaces,
                Clubs = userClubs,
            };

            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var user = await _dashboardRepository.GetUserWithAddressByIdAsync(currentUserId);
            MapUserOntoViewModel(user, dashboardViewModel);
            
            return View(dashboardViewModel);
        }

        public async Task<IActionResult> EditUserProfilePicture()
        {
            // Create new viewModel to receive image file
            var editUserDashboardViewModel = new EditUserDashboardProfilePictureViewModel();

            return View(editUserDashboardViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfilePicture(EditUserDashboardProfilePictureViewModel editProfilePicViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile picture");
                return View("EditUserProfilePicture", editProfilePicViewModel);
            }

            // Find user
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (currentUserId == null)
            {
                return View("Error");
            }
            var user = await _dashboardRepository.GetUserByIdNoTrackingAsync(currentUserId);
            if (user == null)
            {
                return View("Error");
            }

            // Delete existing photo
            if (user.ProfileImagePublicId != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImagePublicId);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(editProfilePicViewModel);
                }
            }

            // Add a new one
            var photoResult = await _photoService.AddPhotoAsync(editProfilePicViewModel.Image);

            user.ProfileImagePublicId = photoResult.PublicId;
            user.ProfileImageUrl = photoResult.Url.ToString();

            _dashboardRepository.Update(user);
            return RedirectToAction("Index", "Dashboard");
        }






        public async Task<IActionResult> EditUserProfileAddress()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();

            if (currentUserId == null)
            {
                return View("Error");
            }
            var user = await _dashboardRepository.GetUserWithAddressByIdAsync(currentUserId);
            if (user == null)
            {
                return View("Error");
            }

            var editUserDashboardAddressViewModel = new EditUserDashboardAddressViewModel()
            {
                Id = currentUserId,
                Street = user.Address?.Street,
                City = user.Address.City,
                County = user.Address.County,
                Postcode = user.Address?.Postcode
            };
            return View(editUserDashboardAddressViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfileAddress(EditUserDashboardAddressViewModel editUserDashboardAddressViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit address");
                return View("EditUserProfileAddress", editUserDashboardAddressViewModel);
            }

            // User may not have an address
            var user = await _dashboardRepository.GetUserWithAddressByIdNoTrackingAsync(editUserDashboardAddressViewModel.Id);
            if (user.Address == null)
            {
                // Address is null
                // Create new Address
                // Add it to user
                var newAddress = new Address
                {
                    Street = editUserDashboardAddressViewModel.Street,
                    City = editUserDashboardAddressViewModel.City,
                    County = editUserDashboardAddressViewModel.County,
                    Postcode = editUserDashboardAddressViewModel.Postcode
                };
                user.Address = newAddress;
            }
            else
            {
                // Address is not null
                // Modify address
                user.Address.Street = editUserDashboardAddressViewModel.Street;
                user.Address.City = editUserDashboardAddressViewModel.City;
                user.Address.County = editUserDashboardAddressViewModel.County;
                user.Address.Postcode = editUserDashboardAddressViewModel.Postcode;
            }

            _dashboardRepository.Update(user);
            return RedirectToAction("Index");
        }
















        public async Task<IActionResult> EditUserProfileStats()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();

            if (currentUserId == null)
            {
                return View("Error");
            }
            var user = await _dashboardRepository.GetUserWithAddressByIdAsync(currentUserId);
            if (user == null)
            {
                return View("Error");
            }

            var editUserDashboardStatsViewModel = new EditUserDashboardStatsViewModel()
            {
                Id = currentUserId,
                Pace = user.Pace,
                Mileage = user.Mileage
            };
            return View(editUserDashboardStatsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfileStats(EditUserDashboardStatsViewModel editUserDashboardStatsViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit stats");
                return View("EditUserProfileStats", editUserDashboardStatsViewModel);
            }

            // User may not have an address
            var user = await _dashboardRepository.GetUserByIdNoTrackingAsync(editUserDashboardStatsViewModel.Id);
            user.Pace = editUserDashboardStatsViewModel.Pace;
            user.Mileage = editUserDashboardStatsViewModel.Mileage;

            _dashboardRepository.Update(user);
            return RedirectToAction("Index");
        }
    }
}
