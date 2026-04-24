namespace SistemaCursos.Models;

public class MiMatriculaItemViewModel
{
    public int MatriculaId { get; set; }
    public int CursoId { get; set; }
    public string CursoCodigo { get; set; } = string.Empty;
    public string CursoNombre { get; set; } = string.Empty;
    public int Creditos { get; set; }
    public EstadoMatricula Estado { get; set; }
    public DateTime FechaMatricula { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public int HoraInicio { get; set; }
    public int HoraFin { get; set; }
}
