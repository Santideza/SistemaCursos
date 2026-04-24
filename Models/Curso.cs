using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCursos.Models;

public class Curso
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código del curso es obligatorio")]
    [StringLength(10, ErrorMessage = "El código no puede tener más de 10 caracteres")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del curso es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede tener más de 200 caracteres")]
    [Display(Name = "Nombre del Curso")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los créditos son obligatorios")]
    [Range(1, 10, ErrorMessage = "Los créditos deben estar entre 1 y 10")]
    [Display(Name = "Créditos")]
    public int Creditos { get; set; }

    [Required(ErrorMessage = "El cupo máximo es obligatorio")]
    [Range(1, 100, ErrorMessage = "El cupo máximo debe estar entre 1 y 100")]
    [Display(Name = "Cupo Máximo")]
    public int CupoMaximo { get; set; }

    [Required(ErrorMessage = "El horario de inicio es obligatorio")]
    [Display(Name = "Horario Inicio")]
    public TimeOnly HorarioInicio { get; set; }

    [Required(ErrorMessage = "El horario de fin es obligatorio")]
    [Display(Name = "Horario Fin")]
    public TimeOnly HorarioFin { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    // Relación con matrículas
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}