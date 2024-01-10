using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Areas.Admin.Models;
using WebApplication1.Entities;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginVM model)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View(model);

            var existingUser = await _userManager.FindByNameAsync(model.Email);

            if (existingUser is null)
            {
                model.ErrorMessage = "Username or password is incorrect!";
                return View(model);
            }

            var result = await _signInManager
                .PasswordSignInAsync(existingUser, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            {
                model.ErrorMessage = "Username or password is incorrect!";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }

                return View();
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
