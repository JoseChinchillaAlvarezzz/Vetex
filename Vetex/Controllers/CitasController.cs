using Microsoft.AspNetCore.Mvc;

namespace Vetex.Controllers
{
    public class CitasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Crear_cita() { 
        return View();
        }
    }
}
