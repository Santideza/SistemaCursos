namespace SistemaCursos.Models;

public class HomeInicioViewModel
{
    public int CursosActivos { get; set; }
    public int CursosConCupo { get; set; }
    public int MatriculasPendientes { get; set; }
    public List<Curso> CursosDestacados { get; set; } = new();
}
