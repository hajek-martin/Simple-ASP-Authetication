using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rozcestnik.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Rozcestnik.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")] // Pouze pro admina
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("Login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // Sem napojení na model LDAP
            if (username == "bob" && password == "pizza")
            {
                var claims = new List<Claim>(); // Popisují usera
                claims.Add(new Claim("username", username)); // Přidám jméno
                claims.Add(new Claim(ClaimTypes.NameIdentifier, username)); // Přidám další
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(ClaimTypes.Role, "Admin")); // Přidám admin (ROLE)
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); //Vytvořím identitu
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity); 
                await HttpContext.SignInAsync(claimsPrincipal);
                if (returnUrl != null) return Redirect(returnUrl);

                return View("Index");
            }
            TempData["Error"] = "Username or password is invalid";
            return View("Login");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}