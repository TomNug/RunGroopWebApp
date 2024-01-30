using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IPhotoService _photoService;
        public AccountController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            ApplicationDbContext context,
            IPhotoService photoService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _photoService = photoService;
        }
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            if (user != null)
            {
                // User found
                // Check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if(passwordCheck)
                {
                    // Password correct
                    // Sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                // Password fails
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginViewModel);
            }
            // No existing user
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginViewModel);
        }


        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);

            if (user != null)
            {
                // Email already associated with a user
                TempData["Error"] = "This email address is already in use";
                return View(registerViewModel);
            }

            // Creating new user

            // Create profile picture by adding to Cloudinary
            var photoResult = await _photoService.AddPhotoAsync(registerViewModel.Image);


            // No existing user with that email
            var newUser = new AppUser()
            {
                Email = registerViewModel.EmailAddress,
                UserName = registerViewModel.Username,
                ProfileImageUrl = photoResult.Url.ToString(),
                ProfileImagePublicId = photoResult.PublicId
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            }
            else
            {
                TempData["Error"] = "Password is not complex enough";
                return View(registerViewModel);
            }

            // Sign in
            var newUserFromDb = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);
            var result = await _signInManager.PasswordSignInAsync(newUserFromDb, registerViewModel.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Register", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
