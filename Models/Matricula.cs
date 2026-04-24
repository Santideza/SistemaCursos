using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SistemaCursos.Models;

public enum EstadoMatricula
{
    Pendiente,
    Activa,
    Cancelada,
    Rechazada
}

public class Matricula
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; }

    [Required]
    public int CursoId { get; set; }

    [ForeignKey("CursoId")]
    public Curso? Curso { get; set; }

    [Required]
    [Display(Name = "Estado")]
    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;

    [Display(Name = "Fecha de Matrícula")]
    public DateTime FechaMatricula { get; set; } = DateTime.Now;

    [Display(Name = "Fecha de Aprobación")]
    public DateTime? FechaAprobacion { get; set; }
}