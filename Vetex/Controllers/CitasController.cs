using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vetex.Models;

namespace Vetex.Controllers
{
    public class CitasController : Controller
    {
        private readonly veterinariaContext _ctx;
        public CitasController(veterinariaContext ctx) => _ctx = ctx;

        // ---------- Helper único: catálogos ----------
        private async Task LoadCatalogsAsync(CrearCitaVM vm)
        {
            vm.Tipos = await _ctx.tipocita
                .OrderBy(t => t.tipo)
                .Select(t => new Opcion { Id = t.id, Texto = t.tipo })
                .ToListAsync();

            vm.Horas = await _ctx.horacita
                .OrderBy(h => h.hora)
                .Select(h => new Opcion { Id = h.id, Texto = h.hora.ToString("h:mm tt") })
                .ToListAsync();

            // No necesario para el autocomplete, pero evita nulls si lo usas en selects
            vm.Mascotas = await (
                from m in _ctx.mascotas
                join d in _ctx.duenos on m.dueno_id equals d.id
                orderby d.nombre, m.nombre
                select new Opcion { Id = m.id, Texto = m.nombre + " — " + d.nombre }
            ).ToListAsync();
        }

        // ---------- LISTA ----------
        public async Task<IActionResult> Index(DateTime? fecha)
        {
            var q =
                from c in _ctx.citas
                join m in _ctx.mascotas on c.mascota_id equals m.id
                join d in _ctx.duenos on m.dueno_id equals d.id
                join h0 in _ctx.horacita on c.horacita_id equals h0.id into hh
                from h in hh.DefaultIfEmpty()
                join t0 in _ctx.tipocita on c.tipocita_id equals t0.id into tt
                from t in tt.DefaultIfEmpty()
                select new
                {
                    c.id,
                    c.fecha,
                    Tipo = t != null ? t.tipo : "(sin tipo)",
                    Dueno = d.nombre,
                    Mascota = m.nombre,
                    Hora = (TimeOnly?)(h != null ? h.hora : null)
                };

            if (fecha.HasValue)
            {
                var dia = fecha.Value.Date;
                q = q.Where(x => x.fecha == dia);
                ViewBag.FiltroTexto = dia.ToString("dd/MM/yyyy");
                ViewBag.FechaFiltro = dia;
            }
            else
            {
                ViewBag.FiltroTexto = "Todas las citas";
                ViewBag.FechaFiltro = null;
            }

            var model = (await q.OrderBy(x => x.fecha).ThenBy(x => x.Hora).ToListAsync())
                .Select(x => new CitaDelDiaVM
                {
                    Codigo = x.id,
                    Fecha = x.fecha,
                    TipoCita = x.Tipo,
                    Dueno = x.Dueno,
                    Mascota = x.Mascota,
                    Hora = x.Hora.HasValue ? x.Hora.Value.ToString("h:mm tt") : ""
                })
                .ToList();

            return View(model);
        }

        // ---------- CREAR (GET) ----------
        [HttpGet]
        public async Task<IActionResult> Crear_cita()
        {
            var vm = new CrearCitaVM { Fecha = DateTime.Today };
            await LoadCatalogsAsync(vm);
            return View(vm);
        }

        // ---------- CREAR (POST) ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear_cita([Bind("MascotaId,TipoCitaId,Fecha,HoraCitaId")] CrearCitaVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadCatalogsAsync(vm);
                return View(vm);
            }

            // Validar FKs
            var mascotaOk = await _ctx.mascotas.AnyAsync(x => x.id == vm.MascotaId);
            var tipoOk = await _ctx.tipocita.AnyAsync(x => x.id == vm.TipoCitaId);
            var horaOk = await _ctx.horacita.AnyAsync(x => x.id == vm.HoraCitaId);

            if (!mascotaOk) ModelState.AddModelError(nameof(vm.MascotaId), "La mascota seleccionada no existe.");
            if (!tipoOk) ModelState.AddModelError(nameof(vm.TipoCitaId), "El tipo de cita no existe.");
            if (!horaOk) ModelState.AddModelError(nameof(vm.HoraCitaId), "La hora seleccionada no existe.");

            if (!ModelState.IsValid)
            {
                await LoadCatalogsAsync(vm);
                return View(vm);
            }

            // Chequeo de choque (fecha + hora)
            var dia = vm.Fecha.Date;
            var hayChoque = await _ctx.citas.AnyAsync(c => c.fecha == dia && c.horacita_id == vm.HoraCitaId);
            if (hayChoque)
            {
                ModelState.AddModelError(string.Empty, "Ya existe una cita en esa fecha y hora.");
                await LoadCatalogsAsync(vm);
                return View(vm);
            }

            var nueva = new citas
            {
                fecha = dia,
                mascota_id = vm.MascotaId!.Value,
                tipocita_id = vm.TipoCitaId!.Value,
                horacita_id = vm.HoraCitaId!.Value
            };

            try
            {
                _ctx.citas.Add(nueva);
                await _ctx.SaveChangesAsync();
                TempData["ok"] = $"Cita creada para el {dia:dd/MM/yyyy}.";
                return RedirectToAction(nameof(Index), new { fecha = dia.ToString("yyyy-MM-dd") });
            }
            catch
            {
                TempData["error"] = "No se pudo crear la cita. Intenta nuevamente.";
                await LoadCatalogsAsync(vm);
                return View(vm);
            }
        }

        // ---------- AUTOCOMPLETE ----------
        [HttpGet]
        public async Task<IActionResult> Buscar(string q)
        {
            q = (q ?? "").Trim();

            var resultados = await (
                from m in _ctx.mascotas
                join d in _ctx.duenos on m.dueno_id equals d.id
                where q == "" || m.nombre.Contains(q) || d.nombre.Contains(q)
                orderby d.nombre, m.nombre
                select new { id = m.id, texto = m.nombre + " — " + d.nombre }
            ).Take(20).ToListAsync();

            return Json(resultados);
        }
    }
}
