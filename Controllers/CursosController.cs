using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SistemaCursos.Data;
using SistemaCursos.Models;
using System.Text.Json;

namespace SistemaCursos.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly UserManager<IdentityUser> _userManager;

    public CursosController(
        ApplicationDbContext context,
        IDistributedCache cache,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _cache = cache;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search, int? creditos)
    {
        var ultimoCursoJson = await _cache.GetStringAsync(ObtenerClaveUltimoCurso());
        if (!string.IsNullOrWhiteSpace(ultimoCursoJson))
        {
            ViewBag.UltimoCursoVisitado = JsonSerializer.Deserialize<UltimoCursoDto>(ultimoCursoJson);
        }

        var query = _context.Cursos
            .AsNoTracking()
            .Where(c => c.Activo);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.Nombre.Contains(search) ||
                c.Codigo.Contains(search));
        }

        if (creditos.HasValue)
        {
            query = query.Where(c => c.Creditos == creditos.Value);
        }

        var cursos = await query
            .Include(c => c.Matriculas)
            .ToListAsync();

        return View(cursos);
    }

    public async Task<IActionResult> Detalle(int? id)
    {
        if (id == null)
            return NotFound();

        var curso = await _context.Cursos
            .AsNoTracking()
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (curso == null)
            return NotFound();

        await GuardarUltimoCursoAsync(curso);

        return View(curso);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Inscribir(int cursoId)
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            TempData["Error"] = "Debes estar autenticado para inscribirte.";
            return RedirectToAction(nameof(Detalle), new { id = cursoId });
        }

        var curso = await _context.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == cursoId);

        if (curso == null)
        {
            TempData["Error"] = "El curso no existe.";
            return RedirectToAction(nameof(Index));
        }

        var yaMatriculado = await _context.Matriculas
            .AnyAsync(m => m.UserId == usuario.Id && m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);

        if (yaMatriculado)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction(nameof(Detalle), new { id = cursoId });
        }

        var matriculasActivas = curso.Matriculas.Count(m => m.Estado != EstadoMatricula.Cancelada);
        if (matriculasActivas >= curso.CupoMaximo)
        {
            TempData["Error"] = $"El curso ha alcanzado su cupo máximo ({curso.CupoMaximo} estudiantes).";
            return RedirectToAction(nameof(Detalle), new { id = cursoId });
        }

        var cursosDelUsuario = await _context.Matriculas
            .Where(m => m.UserId == usuario.Id && m.Estado != EstadoMatricula.Cancelada)
            .Include(m => m.Curso)
            .ToListAsync();

        bool hayConflicto = cursosDelUsuario.Any(m =>
        {
            var cursoExistente = m.Curso;
            if (cursoExistente == null)
            {
                return false;
            }

            return curso.HoraInicio < cursoExistente.HoraFin && curso.HoraFin > cursoExistente.HoraInicio;
        });

        if (hayConflicto)
        {
            TempData["Error"] = "Ya tienes inscripciones en otro curso durante este horario.";
            return RedirectToAction(nameof(Detalle), new { id = cursoId });
        }

        var matricula = new Matricula
        {
            UserId = usuario.Id,
            CursoId = cursoId,
            Estado = EstadoMatricula.Pendiente,
            FechaMatricula = DateTime.Now
        };

        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();

        TempData["Success"] = "¡Te has inscrito correctamente! Tu solicitud está en estado Pendiente.";
        return RedirectToAction(nameof(Detalle), new { id = cursoId });
    }

    private async Task GuardarUltimoCursoAsync(Curso curso)
    {
        var ultimoCurso = new UltimoCursoDto
        {
            Id = curso.Id,
            Nombre = curso.Nombre
        };

        await _cache.SetStringAsync(
            ObtenerClaveUltimoCurso(),
            JsonSerializer.Serialize(ultimoCurso),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
    }

    private string ObtenerClaveUltimoCurso()
    {
        return $"ultimo-curso:{HttpContext.Session.Id}";
    }
}
