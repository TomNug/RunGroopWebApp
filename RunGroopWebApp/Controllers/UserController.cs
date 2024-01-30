using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

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

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();

            var users = await _userRepository.GetAllUsersAsync();
            List<UserViewModel> result = new List<UserViewModel>();
            foreach(var user in users)
            {
                if (currentUserId !=  user.Id)
                {
                    var userViewModel = new UserViewModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Pace = user.Pace,
                        Mileage = user.Mileage,
                        ProfilePictureUrl = user.ProfileImageUrl,
                        City = user.City
                    };
                    result.Add(userViewModel);
                }
            }
            return View(result);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var userDetailViewModel = new UserDetailViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfilePictureUrl = user.ProfileImageUrl,
                City = user.City,
                County = user.County
            };
            return View(userDetailViewModel);
        }
    }
}
