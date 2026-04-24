using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCursos.Data;
using SistemaCursos.Models;

namespace SistemaCursos.Controllers;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CursosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ✅ LISTA + FILTRO
    public async Task<IActionResult> Index(string? search, int? creditos)
    {
        var query = _context.Cursos
            .AsNoTracking() // 🔥 mejora rendimiento
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

    // ✅ DETALLE
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

        return View(curso);
    }

    // ✅ INSCRIBIR
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Inscribir(int cursoId)
    {
        // Obtener usuario autenticado
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            TempData["Error"] = "Debes estar autenticado para inscribirte.";
            return RedirectToAction(nameof(Detalle), new { id = cursoId });
        }

        // Obtener curso
        var curso = await _context.Cursos
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.Id == cursoId);

        if (curso == null)
        {
            TempData["Error"] = "El curso no existe.";
            return RedirectToAction(nameof(Index));
        }

        // Validación 1: Verificar si el usuario ya está matriculado
        var yaMatriculado = await _context.Matriculas
            .AnyAsync(m => m.UserId == usuario.Id && m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);

        if (yaMatriculado)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction(nameof(Detalle), new { id = cursoId });
        }

        // Validación 2: No superar el cupo máximo
        var matriculasActivas = curso.Matriculas.Count(m => m.Estado != EstadoMatricula.Cancelada);
        if (matriculasActivas >= curso.CupoMaximo)
        {
            TempData["Error"] = $"El curso ha alcanzado su cupo máximo ({curso.CupoMaximo} estudiantes).";
            return RedirectToAction(nameof(Detalle), new { id = cursoId });
        }

        // Validación 3: No solaparse con otro curso en el mismo horario
        var cursosDelUsuario = await _context.Matriculas
            .Where(m => m.UserId == usuario.Id && m.Estado != EstadoMatricula.Cancelada)
            .Include(m => m.Curso)
            .ToListAsync();

        bool hayConflicto = cursosDelUsuario.Any(m =>
        {
            var cursoExistente = m.Curso;
            if (cursoExistente == null) return false;
            // Verificar si los horarios se solapan
            return (curso.HoraInicio < cursoExistente.HoraFin && curso.HoraFin > cursoExistente.HoraInicio);
        });

        if (hayConflicto)
        {
            TempData["Error"] = "Ya tienes inscripciones en otro curso durante este horario.";
            return RedirectToAction(nameof(Detalle), new { id = cursoId });
        }

        // Crear nueva matrícula
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
}