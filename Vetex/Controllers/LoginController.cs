using Microsoft.AspNetCore.Mvc;

namespace Vetex.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult menuInicial() 
        {
            return View();
        }
    }
}
