using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;
using System.Runtime.InteropServices.Marshalling;

namespace RunGroopWebApp.Controllers
{
    
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> Index()
        {
            var currentUserId = "";
            if (_httpContextAccessor.HttpContext?.User?.Identity?.Name != null)
            {
                currentUserId = _httpContextAccessor.HttpContext?.User?.GetUserId();
            }

            var users = await _userRepository.GetAllUsersWithAddressAsync();
            List<UserViewModel> result = new List<UserViewModel>();
            foreach(var user in users)
            {
                if (currentUserId != user.Id)
                {
                    var userViewModel = new UserViewModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Pace = user.Pace,
                        Mileage = user.Mileage,
                        ProfilePictureUrl = user.ProfileImageUrl,
                        City = user.Address?.City
                    };
                    result.Add(userViewModel);
                }
            }
            return View(result);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetUserByIdWithAddressAsync(id);

            List<AppUser> relatedUsers = new List<AppUser>();

            if (user.Address != null)
            {
                relatedUsers = await _userRepository.GetNUsersByCityIncludingAddressExcludingIdAsync(3, user.Address.City, user.Id);
            }

            var userDetailViewModel = new UserDetailViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfilePictureUrl = user.ProfileImageUrl,
                City = user.Address?.City,
                County = user.Address?.County,
                RelatedUsers = relatedUsers
            };
            return View(userDetailViewModel);
        }
    }
}
