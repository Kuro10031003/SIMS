using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIMS.Data;
using SIMS.Models;

namespace SIMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _hasher = new();

        public AccountController(AppDbContext db) { _db = db; }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.IsActive);
            if (user == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // verify the typed password against the stored HASH (never plain text)
            var check = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (check == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // build the login session with the user's role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            // RBAC: send each role to its own home page
            return user.Role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Lecturer" => RedirectToAction("Index", "Lecturer"),
                "Student" => RedirectToAction("Index", "Student"),
                _ => RedirectToAction("Login")
            };
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}