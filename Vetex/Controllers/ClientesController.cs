using Microsoft.AspNetCore.Mvc;

namespace Vetex.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            return View();

        }
        public IActionResult Registro_Dueno()
        {
            return View();
        }
        public IActionResult Editar_Datos()
        {
            return View();
        }
    }
}
