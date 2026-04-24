namespace SistemaCursos.Models;

public class CoordinadorDashboardViewModel
{
    public int TotalCursos { get; set; }
    public int CursosActivos { get; set; }
    public int MatriculasPendientes { get; set; }
    public List<CoordinadorCursoResumenViewModel> Cursos { get; set; } = new();
}
