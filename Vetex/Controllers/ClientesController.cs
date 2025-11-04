using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vetex.Models;

namespace Vetex.Controllers
{
    public class ClientesController : Controller
    {
        private readonly veterinariaContext _ctx;

        public ClientesController(veterinariaContext ctx)
        {
            _ctx = ctx;
        }

        // =================== LISTADO ===================

        // GET: /Clientes?q=...
        public async Task<IActionResult> Index(string? q)
        {
            var duenosBase = _ctx.duenos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                duenosBase = duenosBase.Where(d =>
                    d.nombre.Contains(q) ||
                    (d.telefono != null && d.telefono.Contains(q)));
            }

            var duenos = await duenosBase
                .OrderBy(d => d.nombre)
                .Select(d => new
                {
                    d.id,
                    d.nombre,
                    d.telefono,
                    Mascotas = (from m in _ctx.mascotas
                                where m.dueno_id == d.id
                                join e in _ctx.especies on m.especie_id equals e.id into ej
                                from e in ej.DefaultIfEmpty()
                                select new
                                {
                                    nombre = m.nombre,
                                    especie = e != null ? e.especie : null
                                    // si luego quieres mostrar la fecha:
                                    // , fecha_nacimiento = m.fecha_nacimiento
                                }).ToList()
                })
                .ToListAsync();

            ViewBag.Q = q;
            return View(duenos);
        }

        // =================== REGISTRO DE DUEÑO ===================

        [HttpGet]
        public IActionResult Registro_Dueno()
        {
            return View(new duenos());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro_Dueno(duenos model)
        {
            model.nombre = (model.nombre ?? "").Trim();
            model.telefono = string.IsNullOrWhiteSpace(model.telefono) ? null : model.telefono.Trim();
            model.dui = (model.dui ?? "").Trim();
            model.direccion = string.IsNullOrWhiteSpace(model.direccion) ? null : model.direccion.Trim();

            if (!ModelState.IsValid) return View(model);

            if (!string.IsNullOrEmpty(model.telefono) &&
                !Regex.IsMatch(model.telefono, @"^\d{4}-\d{4}$"))
            {
                ModelState.AddModelError(nameof(model.telefono), "El teléfono debe tener el formato ####-####.");
                return View(model);
            }

            if (!Regex.IsMatch(model.dui, @"^\d{8}-\d$"))
            {
                ModelState.AddModelError(nameof(model.dui), "El DUI debe tener el formato ########-#.");
                return View(model);
            }

            var existsDui = await _ctx.duenos.AnyAsync(d => d.dui == model.dui);
            if (existsDui)
            {
                ModelState.AddModelError(nameof(model.dui), "Ya existe un dueño con este DUI.");
                return View(model);
            }

            _ctx.duenos.Add(model);
            await _ctx.SaveChangesAsync();

            TempData["msg"] = "Dueño registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =================== EDICIÓN DE DUEÑO ===================

        [HttpGet]
        public async Task<IActionResult> Editar_Datos(int id)
        {
            var dueno = await _ctx.duenos.FindAsync(id);
            if (dueno == null) return NotFound();
            return View(dueno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar_Datos(int id, duenos model)
        {
            if (id != model.id) return BadRequest();

            model.nombre = (model.nombre ?? "").Trim();
            model.telefono = string.IsNullOrWhiteSpace(model.telefono) ? null : model.telefono.Trim();
            model.dui = (model.dui ?? "").Trim();
            model.direccion = string.IsNullOrWhiteSpace(model.direccion) ? null : model.direccion.Trim();

            if (!ModelState.IsValid) return View(model);

            if (!string.IsNullOrEmpty(model.telefono) &&
                !Regex.IsMatch(model.telefono, @"^\d{4}-\d{4}$"))
            {
                ModelState.AddModelError(nameof(model.telefono), "El teléfono debe tener el formato ####-####.");
                return View(model);
            }

            if (!Regex.IsMatch(model.dui, @"^\d{8}-\d$"))
            {
                ModelState.AddModelError(nameof(model.dui), "El DUI debe tener el formato ########-#.");
                return View(model);
            }

            var existsDui = await _ctx.duenos.AnyAsync(d => d.id != id && d.dui == model.dui);
            if (existsDui)
            {
                ModelState.AddModelError(nameof(model.dui), "Ya existe otro dueño con este DUI.");
                return View(model);
            }

            try
            {
                _ctx.Entry(model).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();

                TempData["msg"] = "Datos actualizados correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "No se pudo actualizar el registro.");
                return View(model);
            }
        }

        // =================== AGREGAR MASCOTA ===================

        private async Task CargarCatalogosAsync()
        {
            var especies = await _ctx.especies
                .OrderBy(e => e.especie)
                .Select(e => new { e.id, e.especie })
                .ToListAsync();

            var sexos = await _ctx.sexos
                .OrderBy(s => s.sexo)
                .Select(s => new { s.id, s.sexo })
                .ToListAsync();

            ViewBag.Especies = new SelectList(especies, "id", "especie");
            ViewBag.Sexos = new SelectList(sexos, "id", "sexo");
        }

        // GET: /Clientes/Agregar_Mascota?duenoId=5
        [HttpGet]
        public async Task<IActionResult> Agregar_Mascota(int duenoId)
        {
            var duenoExiste = await _ctx.duenos.AnyAsync(d => d.id == duenoId);
            if (!duenoExiste) return NotFound();

            await CargarCatalogosAsync();
            var model = new mascotas
            {
                dueno_id = duenoId,
                fecha_nacimiento = DateTime.Today // valor por defecto
            };
            return View(model);
        }

        // POST: /Clientes/Agregar_Mascota
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar_Mascota(mascotas model)
        {
            // Dueño válido
            if (model.dueno_id <= 0 || !await _ctx.duenos.AnyAsync(d => d.id == model.dueno_id))
            {
                ModelState.AddModelError("", "Dueño no válido.");
            }

            // Reglas nuevas: nombre obligatorio, especie y sexo seleccionados,
            // fecha de nacimiento no futura, peso no negativo.
            if (string.IsNullOrWhiteSpace(model.nombre))
            {
                ModelState.AddModelError(nameof(model.nombre), "El nombre de la mascota es obligatorio.");
            }

            if (model.especie_id <= 0)
            {
                ModelState.AddModelError(nameof(model.especie_id), "Seleccione la especie.");
            }

            if (model.sexo_id <= 0)
            {
                ModelState.AddModelError(nameof(model.sexo_id), "Seleccione el sexo.");
            }

            if (model.fecha_nacimiento == default)
            {
                ModelState.AddModelError(nameof(model.fecha_nacimiento), "Seleccione la fecha de nacimiento.");
            }
            else if (model.fecha_nacimiento.Date > DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.fecha_nacimiento), "La fecha de nacimiento no puede ser futura.");
            }

            if (model.peso < 0)
            {
                ModelState.AddModelError(nameof(model.peso), "El peso no puede ser negativo.");
            }

            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync();
                return View(model);
            }

            _ctx.mascotas.Add(model);
            await _ctx.SaveChangesAsync();

            TempData["msg"] = "Mascota agregada correctamente.";
            return RedirectToAction(nameof(Editar_Datos), new { id = model.dueno_id });
        }
    }
}
