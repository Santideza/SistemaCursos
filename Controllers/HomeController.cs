using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCursos.Data;
using SistemaCursos.Models;

namespace SistemaCursos.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var cursosDestacados = await _context.Cursos
            .AsNoTracking()
            .Where(c => c.Activo)
            .Include(c => c.Matriculas)
            .OrderBy(c => c.HoraInicio)
            .Take(3)
            .ToListAsync();

        var model = new HomeInicioViewModel
        {
            CursosActivos = await _context.Cursos.CountAsync(c => c.Activo),
            CursosConCupo = await _context.Cursos
                .CountAsync(c => c.Activo && c.Matriculas.Count(m => m.Estado != EstadoMatricula.Cancelada) < c.CupoMaximo),
            MatriculasPendientes = await _context.Matriculas.CountAsync(m => m.Estado == EstadoMatricula.Pendiente),
            CursosDestacados = cursosDestacados
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
