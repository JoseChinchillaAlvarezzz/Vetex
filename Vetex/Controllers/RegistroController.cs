using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vetex.Models;

namespace Vetex.Controllers
{
    public class RegistroController : Controller
    {
        private readonly veterinariaContext _context;

        public RegistroController(veterinariaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List(string? ownerName, string? petName)
        {
            var filas = await (from d in _context.duenos
                               join m in _context.mascotas on d.id equals m.dueno_id
                               where (string.IsNullOrEmpty(ownerName) || d.nombre.Contains(ownerName))
                                  && (string.IsNullOrEmpty(petName)  || m.nombre.Contains(petName))
                               orderby d.nombre, m.nombre
                               select new OwnerPetRowVM
                               {
                                   DuenoId   = d.id,
                                   Dueno     = d.nombre,
                                   MascotaId = m.id,
                                   Mascota   = m.nombre,
                                   Telefono  = d.telefono,
                                   Dui       = d.dui
                               })
                               .ToListAsync();

            var vm = new RegistroListaVM { OwnerName = ownerName, PetName = petName, Filas = filas };
            return View(vm);
        }




        [HttpGet]
        public async Task<IActionResult> History(int duenoId, int mascotaId)
        {
            var header = await (from d in _context.duenos
                                join m in _context.mascotas on d.id equals m.dueno_id
                                where d.id == duenoId && m.id == mascotaId
                                select new { Dueno = d.nombre, Mascota = m.nombre })
                                .FirstOrDefaultAsync();
            if (header == null) return NotFound();

            var regs = await (from r in _context.registro
                              where r.mascota_id == mascotaId
                              orderby r.id descending              // <— ordena por ID (últimos arriba)
                              select new HistorialItemVM
                              {
                                  RegistroId  = r.id,
                                  Fecha       = null,               // <— QUITA el EF.Property
                                  Diagnostico = r.diagnostico
                              }).ToListAsync();

            var vm = new HistorialVM
            {
                DuenoId  = duenoId,
                MascotaId= mascotaId,
                Dueno    = header.Dueno,
                Mascota  = header.Mascota,
                Registros= regs
            };
            return View(vm);
        }


        // DETALLE de un registro
        [HttpGet]
        public async Task<IActionResult> Details(int id, int? duenoId, int? mascotaId)
        {
            var head = await (from r in _context.registro
                              join m in _context.mascotas on r.mascota_id equals m.id
                              join d in _context.duenos on m.dueno_id equals d.id
                              join pu in _context.pulso on r.pulso_id equals pu.id
                              join re in _context.respiracion on r.respiracion_id equals re.id
                              join de in _context.deshidratacion on r.deshidratacion_id equals de.id
                              where r.id == id
                              select new
                              {
                                  r.id,
                                  Dueno = d.nombre,
                                  DuenoId = d.id,
                                  Mascota = m.nombre,
                                  MascotaId = m.id,
                                  r.resena,
                                  r.motivo,
                                  r.temperatura,
                                  Pulso = pu.nivel,
                                  Respiracion = re.nivel,
                                  Deshidratacion = de.nivel,
                                  r.diagnostico
                              }).FirstOrDefaultAsync();
            if (head == null) return NotFound();

            var pres = await (from p in _context.prescripcion
                              join med in _context.medicamentos on p.medicamento_id equals med.id
                              join f in _context.frecuencias on p.frecuencia_id equals f.id
                              where p.registro_id == id
                              select new PrescItemVM
                              {
                                  Medicamento = med.nombre + " " + med.concentracion,
                                  Cantidad = p.cantidad,
                                  Frecuencia = f.frecuencia
                              }).ToListAsync();

            var vm = new RegistroDetalleVM
            {
                RegistroId = head.id,
                Dueno = head.Dueno,
                Mascota = head.Mascota,
                Resena = head.resena,
                Motivo = head.motivo,
                Temperatura = head.temperatura,
                Pulso = head.Pulso,
                Respiracion = head.Respiracion,
                Deshidratacion = head.Deshidratacion,
                Diagnostico = head.diagnostico,
                Prescripciones = pres,
                // prioriza lo que venga por query, si no usa lo que sacamos del join
                DuenoId = duenoId ?? head.DuenoId,
                MascotaId = mascotaId ?? head.MascotaId
            };
            return View(vm);
        }


        [HttpGet]
        public IActionResult Create()
        {
            CargarCombos();

            var meds = (from m in _context.medicamentos
                        join pr in _context.presentacionmedicina on m.presentacion_id equals pr.id
                        orderby m.nombre
                        select new
                        {
                            id = m.id,
                            texto = m.nombre + " " + m.concentracion + " — " + pr.presentacion
                        }).ToList();

            ViewBag.Meds = meds; 

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistroCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(model.MascotaId, model.PulsoId, model.RespiracionId, model.DeshidratacionId);
                ViewBag.Meds = (from m in _context.medicamentos
                                join pr in _context.presentacionmedicina on m.presentacion_id equals pr.id
                                orderby m.nombre
                                select new { id = m.id, texto = m.nombre + " " + m.concentracion + " — " + pr.presentacion })
                                .ToList();
                return View(model);
            }

            var reg = new registro
            {
                mascota_id        = model.MascotaId,
                resena            = model.Resena ?? string.Empty,
                motivo            = model.Motivo ?? string.Empty,
                temperatura       = model.Temperatura,
                pulso_id          = model.PulsoId,
                respiracion_id    = model.RespiracionId,
                deshidratacion_id = model.DeshidratacionId,
                diagnostico       = model.Diagnostico ?? string.Empty,
                pagadoaqui        = null
            };

            _context.registro.Add(reg);
            await _context.SaveChangesAsync(); 

            if (model.Medicamentos?.Count > 0)
            {
                var ids = model.Medicamentos.Select(x => x.MedicamentoId).ToList();
                var medsValidos = await _context.medicamentos
                    .Where(m => ids.Contains(m.id))
                    .Select(m => m.id)
                    .ToListAsync();

                foreach (var item in model.Medicamentos)
                {
                    if (!medsValidos.Contains(item.MedicamentoId)) continue;
                    _context.prescripcion.Add(new prescripcion
                    {
                        registro_id    = reg.id,
                        medicamento_id = item.MedicamentoId,
                        dosis          = item.Dosis ?? string.Empty, 
                        cantidad       = item.Cantidad,              
                        frecuencia_id  = 1,                          
                        duracion       = string.Empty,
                        indicacion     = string.Empty
                    });

                }

                await _context.SaveChangesAsync();

            }
            TempData["SuccessMessage"] = "¡Registro guardado con éxito!";
            return RedirectToAction("Index");
        }



        private void CargarCombos(int? mascotaSel = null, int? pulsoSel = null, int? respSel = null, int? deshiSel = null)
        {
            var mascotas = (from m in _context.mascotas
                            join d in _context.duenos on m.dueno_id equals d.id
                            select new { m.id, Texto = d.nombre + " — " + m.nombre })
                           .OrderBy(x => x.Texto).ToList();

            var pulsos = _context.pulso
                           .Select(p => new { p.id, Texto = p.nivel })
                           .OrderBy(p => p.Texto).ToList();

            var respiraciones = _context.respiracion
                           .Select(r => new { r.id, Texto = r.nivel })
                           .OrderBy(r => r.Texto).ToList();

            var deshidrataciones = _context.deshidratacion
                           .Select(d => new { d.id, Texto = d.nivel })
                           .OrderBy(d => d.Texto).ToList();

            ViewBag.Mascotas = new SelectList(mascotas, "id", "Texto", mascotaSel);
            ViewBag.Pulsos = new SelectList(pulsos, "id", "Texto", pulsoSel);
            ViewBag.Respiraciones = new SelectList(respiraciones, "id", "Texto", respSel);
            ViewBag.Deshidrataciones = new SelectList(deshidrataciones, "id", "Texto", deshiSel);
        }

    }
}
