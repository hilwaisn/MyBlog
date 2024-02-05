using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using MyBlog.Migrations;
using MyBlog.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MyBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
		public AccountController(AppDbContext context)
		{
			_context = context;
		}
        public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login([FromForm] Login data)
		{
            var userFromDb = _context.User.FirstOrDefault(x => x.Username == data.Username && x.Password == data.Password);
            if (userFromDb == null)
			{
				@ViewBag.Error = "User not found";
				return View();
			}

			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, data.Username),
				new Claim(ClaimTypes.Role, "Admin"),
			};
			var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
			var identity = new ClaimsIdentity(claims, scheme);

			await HttpContext.SignInAsync(scheme, new ClaimsPrincipal(identity));
			return RedirectToAction("Index", "Home");
		}
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			return RedirectToAction("Login");
		}
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] Registrasi data)
        {
            var user = new User
            {
                Name = data.Name,
                Username = data.Username,
                Email = data.Email,
                Password = data.Password,
                Address = data.Address,
                Phone = data.Phone,
                DateOfBirth = data.DateOfBirth,
                Role = "Admin"
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, data.Username),
                    new Claim(ClaimTypes.Role, "Admin"),
                };

            var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            var identity = new ClaimsIdentity(claims, scheme);

            await HttpContext.SignInAsync(scheme, new ClaimsPrincipal(identity));
            return RedirectToAction("Index", "Home");
        }
		public IActionResult Show(int page = 1)
		{
			ViewBag.PrevPage = (page > 1) ? page - 1 : 1;
			ViewBag.NextPage = page + 1;
			int dataPerpage = 10;
			int skip = dataPerpage * page - dataPerpage;
			List<User> data = _context.User.ToList();
			List<User> filtreddata = data
				.Skip(skip)
				.Take(dataPerpage)
				.OrderBy(user => user.Username)
				.ToList();
			return View(filtreddata);
		}
		public IActionResult Detail(int id)
		{
			User data = _context.User.Where(User => User.Id == id).FirstOrDefault();
			return View(data);
		}
	}
}
