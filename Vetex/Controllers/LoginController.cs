using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vetex.Models;

namespace Vetex.Controllers
{
    public class LoginController : Controller
    {
        private readonly veterinariaContext _context;

        public LoginController(veterinariaContext context)
        {
            _context = context;
        }

        private static string HashSha256Base64(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }


        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") != null)
                return RedirectToAction("menuInicial", "Login");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string usuario, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Ingrese usuario y contraseña.";
                return View();
            }

            
            var hashIngresado = HashSha256Base64(contrasena);

            
            var user = await _context.Set<usuarios>()
                                     .FirstOrDefaultAsync(u =>
                                         u.usuario == usuario &&
                                         u.contrasenia == hashIngresado);

            if (user == null)
            {
                ViewBag.Error = "Usuario o contraseña inválidos.";
                return View();
            }

            
            HttpContext.Session.SetInt32("UsuarioId", user.id);
            HttpContext.Session.SetString("UsuarioNombre", user.usuario);

            
            return RedirectToAction("menuInicial", "Login");
        }

        
        [HttpGet]
        public IActionResult menuInicial()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
                return RedirectToAction("Index", "Login");

            ViewBag.UsuarioNombre = HttpContext.Session.GetString("UsuarioNombre");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        

    }
}
