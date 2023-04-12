using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MeinProfile.Models;

namespace MeinProfile.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _hosting;
        private readonly AppIdentityDbContext _context;

        public AccountController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            IWebHostEnvironment hosting,
            AppIdentityDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _hosting = hosting;
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterView usr)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadImage(usr.ImagePath);
                AppUser user = new AppUser
                {
                    Fullname = usr.Fullname,
                    UserName = usr.Username,
                    Email = usr.Email,
                    City = usr.City,
                    Path = uniqueFileName,
                    PasswordHash = usr.Password

                };
                var result = await _userManager.CreateAsync(user, usr.Password);
                return RedirectToAction("Index", "Account");
            }
            return View(usr);
        }
        [AllowAnonymous]
        public async Task<IActionResult> People()
        {
            var people = await _userManager.Users
                .Select(o => new { Fullname = o.Fullname,  City = o.City })
                .ToListAsync();

            var peopleList = people.Cast<object>().ToList();
            return View(peopleList);
        }
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginView usr, string? returnUrl)
        {
            var usrapp = _userManager.FindByNameAsync(usr.Username).GetAwaiter().GetResult();
            if (usrapp != null)
            {
                _signInManager.SignOutAsync().GetAwaiter().GetResult();
                var hasil = _signInManager.PasswordSignInAsync(usrapp, usr.Password, false, false).GetAwaiter().GetResult();
                if (hasil.Succeeded)
                    return RedirectToAction("Index", "User");
            }
            return View(usr);
        }
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().GetAwaiter().GetResult();
            return Redirect("/home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }



        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = string.Empty;
            if (file != null)
            {
                string uploadFolder = _hosting.WebRootPath + "/upload/";
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
