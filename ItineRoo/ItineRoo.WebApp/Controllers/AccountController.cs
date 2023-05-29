using ItineRoo.WebAPI.Interfaces;
using ItineRoo.WebAPI.Models;
using ItineRoo.WebApp.Interfaces;
using ItineRoo.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ItineRoo.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new AccountModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountModel accountModel)
        {
            var apiResult = await _accountService.RegisterAccount(accountModel.UserRegisterModel);

            if (apiResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return View("Error", new ErrorViewModel());
            }

            return RedirectToAction("Verify");
        }

        [HttpGet]
        public IActionResult Verify()
        {
            return View(new AccountModel());
        }

        [HttpPost]
        public async Task<IActionResult> Verify(AccountModel accountModel)
        {
            var apiResult = await _accountService.VerifyUser(accountModel.UserVerificationModel);

            if (apiResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return View("Error", new ErrorViewModel());
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View(new AccountModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountModel accountModel)
        {
            var apiResult = await _accountService.LoginAccount(accountModel.UserLoginModel);

            if (apiResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return View("Error", new ErrorViewModel());
            }

            // if api authenticated and user logged in
            // Create claims here?

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, accountModel.UserLoginModel.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true,

            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);


            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("login");
        }
    }
}
