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
}