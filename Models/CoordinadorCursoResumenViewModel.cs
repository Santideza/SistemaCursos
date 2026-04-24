namespace SistemaCursos.Models;

public class CoordinadorCursoResumenViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Creditos { get; set; }
    public int CupoMaximo { get; set; }
    public int HoraInicio { get; set; }
    public int HoraFin { get; set; }
    public bool Activo { get; set; }
    public int MatriculasActivas { get; set; }
    public int MatriculasPendientes { get; set; }
}
