using Microsoft.AspNetCore.Mvc;
using Vetex.Models;

namespace Vetex.Controllers
{
    public class inventarioController : Controller
    {
        private readonly veterinariaContext _context;

        public inventarioController(veterinariaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Facturacion()
        {
            var facturasPendientes = (from m in _context.mascotas
                                      join d in _context.duenos
                                            on m.dueno_id equals d.id
                                      join r in _context.registro
                                            on m.id equals r.mascota_id
                                      where r.pagadoaqui == null
                                      select new RegistroPendienteViewModel
                                      {
                                          ID = r.id,
                                          Dueno = d.nombre,
                                          Mascota = m.nombre
                                      }).ToList();

            ViewData["facturasPendientes"] = facturasPendientes;

            return View();
        }

        public IActionResult Inventariado() 
        {
            var medicamentos = (from m in _context.medicamentos
                                join pr in _context.presentacionmedicina
                                        on m.presentacion_id equals pr.id
                                select new 
                                {
                                    id = m.id,
                                    medicamento = m.nombre +" "+ m.concentracion,
                                    presentacion = pr.presentacion,
                                    stock = m.stock
                                }).ToList();

            ViewData["listaMedicamentos"] = medicamentos;

            return View();
        }

        public IActionResult verPrescripciones(int idRegistro)
        {
            var datosCliente = (from m in _context.mascotas
                                join d in _context.duenos
                                        on m.dueno_id equals d.id
                                join r in _context.registro
                                        on m.id equals r.mascota_id
                                where r.id == idRegistro
                                select new
                                {
                                    idRegistro = idRegistro,
                                    idDueno = d.id,
                                    dueno = d.nombre,
                                    mascota = m.nombre
                                }).FirstOrDefault();

            ViewData["datosCliente"] = datosCliente;

            var prescripciones = (from m in _context.medicamentos
                                  join p in _context.prescripcion
                                        on m.id equals p.medicamento_id
                                  where p.registro_id == idRegistro
                                  select new
                                  {
                                      medicamento = m.nombre,
                                      precio = m.precio,
                                      cantidad = p.cantidad, 
                                      total = Math.Round(m.precio * p.cantidad, 2) 
                                  }).ToList();

            ViewData["prescripciones"] = prescripciones;

            return View();
        }

        public IActionResult detalleMedicamento(int idMed, string nombre, string presentacion, int stockActual) 
        {
            var medicamento = new { id = idMed, nombre = nombre, presentacion = presentacion, stock = stockActual};

            ViewData["medicamento"] = medicamento;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Pago(int idRegistro, bool pagadoAqui, int? idDueno, decimal? total) 
        {
            var registro = (from r in _context.registro 
                            where r.id == idRegistro select r).FirstOrDefault();

            if (registro != null) 
            {
                registro.pagadoaqui = pagadoAqui;

                if (pagadoAqui && idDueno.HasValue && total.HasValue) 
                {
                    var factura = new facturas
                    {
                        dueno_id = idDueno.Value,
                        fecha = DateTime.Now,
                        total = total.Value
                    };

                    _context.facturas.Add(factura);
                    await _context.SaveChangesAsync();

                    int idFactura = factura.id;

                    var prescripciones = (from p in _context.prescripcion
                                          join m in _context.medicamentos
                                                on p.medicamento_id equals m.id
                                          where p.registro_id == idRegistro select new 
                                          {
                                              id = p.id,
                                              precio = m.precio,
                                              cantidad = p.cantidad, 
                                              medicamento_id = m.id
                                          }).ToList();

                    foreach (var p in prescripciones) 
                    {
                        var detalle = new detallefactura 
                        {
                            factura_id = idFactura,
                            prescripcion_id = p.id,
                            subtotal = (p.precio * p.cantidad)
                        };

                        _context.detallefactura.Add(detalle);

                        var medicamento = (from m in _context.medicamentos
                                           where m.id == p.medicamento_id
                                           select m).FirstOrDefault();

                        if (medicamento != null) 
                        {
                            medicamento.stock -= p.cantidad;

                            if (medicamento.stock < 0) medicamento.stock = 0;
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                else 
                {
                    await _context.SaveChangesAsync();
                }
                

            }

            return RedirectToAction("Facturacion");
        }

        [HttpPost]
        public async Task<IActionResult> actualizarStock(int idMedicamento, int stockSumar) 
        {
            var medicamento = (from m in _context.medicamentos
                               where m.id == idMedicamento 
                               select m).FirstOrDefault();

            if (medicamento == null) return NotFound("El medicamento no fue encontrado."); 
                
            medicamento.stock = medicamento.stock + stockSumar; 
            _context.SaveChanges();


            return RedirectToAction("Inventariado");
        }

    }
}
