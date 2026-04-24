using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCursos.Data;
using SistemaCursos.Models;

namespace SistemaCursos.Controllers;

[Authorize(Roles = "Coordinador")]
[Route("Coordinador")]
public class CoordinadorController : Controller
{
    private readonly ApplicationDbContext _context;

    public CoordinadorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var cursos = await _context.Cursos
            .AsNoTracking()
            .Include(c => c.Matriculas)
            .OrderBy(c => c.Activo ? 0 : 1)
            .ThenBy(c => c.Nombre)
            .Select(c => new CoordinadorCursoResumenViewModel
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Nombre = c.Nombre,
                Creditos = c.Creditos,
                CupoMaximo = c.CupoMaximo,
                HoraInicio = c.HoraInicio,
                HoraFin = c.HoraFin,
                Activo = c.Activo,
                MatriculasActivas = c.Matriculas.Count(m => m.Estado == EstadoMatricula.Activa),
                MatriculasPendientes = c.Matriculas.Count(m => m.Estado == EstadoMatricula.Pendiente)
            })
            .ToListAsync();

        var model = new CoordinadorDashboardViewModel
        {
            TotalCursos = cursos.Count,
            CursosActivos = cursos.Count(c => c.Activo),
            MatriculasPendientes = cursos.Sum(c => c.MatriculasPendientes),
            Cursos = cursos
        };

        return View(model);
    }

    [HttpGet("Crear")]
    public IActionResult Crear()
    {
        return View("FormularioCurso", new Curso
        {
            Activo = true
        });
    }

    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Curso curso)
    {
        if (await _context.Cursos.AnyAsync(c => c.Codigo == curso.Codigo))
        {
            ModelState.AddModelError(nameof(Curso.Codigo), "Ya existe un curso con ese código.");
        }

        if (!ModelState.IsValid)
        {
            return View("FormularioCurso", curso);
        }

        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Curso creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
        {
            return NotFound();
        }

        return View("FormularioCurso", curso);
    }

    [HttpPost("Editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Curso curso)
    {
        if (id != curso.Id)
        {
            return NotFound();
        }

        if (await _context.Cursos.AnyAsync(c => c.Codigo == curso.Codigo && c.Id != curso.Id))
        {
            ModelState.AddModelError(nameof(Curso.Codigo), "Ya existe un curso con ese código.");
        }

        if (!ModelState.IsValid)
        {
            return View("FormularioCurso", curso);
        }

        _context.Update(curso);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Curso actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Desactivar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
        {
            return NotFound();
        }

        curso.Activo = false;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Curso desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Matriculas/{cursoId:int}")]
    public async Task<IActionResult> Matriculas(int cursoId)
    {
        var curso = await _context.Cursos
            .AsNoTracking()
            .Include(c => c.Matriculas)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == cursoId);

        if (curso == null)
        {
            return NotFound();
        }

        var model = new CoordinadorCursoMatriculasViewModel
        {
            CursoId = curso.Id,
            CursoCodigo = curso.Codigo,
            CursoNombre = curso.Nombre,
            CupoMaximo = curso.CupoMaximo,
            MatriculasActivas = curso.Matriculas.Count(m => m.Estado == EstadoMatricula.Activa),
            Matriculas = curso.Matriculas
                .OrderBy(m => m.Estado)
                .ThenBy(m => m.FechaMatricula)
                .Select(m => new CoordinadorMatriculaViewModel
                {
                    Id = m.Id,
                    EstudianteEmail = m.User?.Email ?? m.UserId,
                    Estado = m.Estado,
                    FechaMatricula = m.FechaMatricula,
                    FechaAprobacion = m.FechaAprobacion
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost("Matriculas/Confirmar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(int id)
    {
        var matricula = await _context.Matriculas
            .Include(m => m.Curso)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (matricula == null || matricula.Curso == null)
        {
            return NotFound();
        }

        var cursoId = matricula.CursoId;
        var activas = await _context.Matriculas.CountAsync(m =>
            m.CursoId == cursoId && m.Estado == EstadoMatricula.Activa);

        if (activas >= matricula.Curso.CupoMaximo)
        {
            TempData["Error"] = "No se puede confirmar la matrícula porque el curso ya alcanzó su cupo máximo.";
            return RedirectToAction(nameof(Matriculas), new { cursoId });
        }

        matricula.Estado = EstadoMatricula.Activa;
        matricula.FechaAprobacion = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Matrícula confirmada correctamente.";
        return RedirectToAction(nameof(Matriculas), new { cursoId });
    }

    [HttpPost("Matriculas/Cancelar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var matricula = await _context.Matriculas.FirstOrDefaultAsync(m => m.Id == id);
        if (matricula == null)
        {
            return NotFound();
        }

        matricula.Estado = EstadoMatricula.Cancelada;
        matricula.FechaAprobacion = null;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Matrícula cancelada correctamente.";
        return RedirectToAction(nameof(Matriculas), new { cursoId = matricula.CursoId });
    }
}
