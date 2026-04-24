namespace SistemaCursos.Models;

public class CoordinadorCursoMatriculasViewModel
{
    public int CursoId { get; set; }
    public string CursoCodigo { get; set; } = string.Empty;
    public string CursoNombre { get; set; } = string.Empty;
    public int CupoMaximo { get; set; }
    public int MatriculasActivas { get; set; }
    public List<CoordinadorMatriculaViewModel> Matriculas { get; set; } = new();
}
