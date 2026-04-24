namespace SistemaCursos.Models;

public class CoordinadorMatriculaViewModel
{
    public int Id { get; set; }
    public string EstudianteEmail { get; set; } = string.Empty;
    public EstadoMatricula Estado { get; set; }
    public DateTime FechaMatricula { get; set; }
    public DateTime? FechaAprobacion { get; set; }
}
