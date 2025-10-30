using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using APP.Data;
using APP.Models;

namespace APP.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var user = await _context.NguoiDung
                .FirstOrDefaultAsync(u => u.tendangnhap.Trim().ToLower() == model.tendangnhap.Trim().ToLower());

            if (user == null)
            {

                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            if (model.matkhau.Trim() != "1")
            {
           
                ModelState.AddModelError(string.Empty, " mật khẩu không đúng");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.tendangnhap)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                });
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }  
        [HttpGet]
        public IActionResult Connect()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Connect(ConnectionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);           
            string connectionString = $"Server={model.Server};Database={model.Database};User Id={model.UserId};Password={model.Password};TrustServerCertificate=True;";

            try
            {
                using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
                {
                    conn.Open();
                }

                HttpContext.Session.SetString("DynamicConnection", connectionString);
                TempData["Success"] = "Kết nối thành công!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kết nối thất bại: " + ex.Message;
                return View(model);
            }
        }        
        public IActionResult TestConnection([FromBody] ConnectionViewModel model)
        {
            string conn = $"Server={model.Server};Database={model.Database};User Id={model.UserId};Password={model.Password};TrustServerCertificate=True;";
            try
            {
                using (var sql = new Microsoft.Data.SqlClient.SqlConnection(conn))
                {
                    sql.Open();
                }

                HttpContext.Session.SetString("DynamicConnection", conn);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}