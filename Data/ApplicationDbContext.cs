using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaCursos.Models;

namespace SistemaCursos.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            modelBuilder.Entity<Curso>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0");
                t.HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");
            });
        }
    }
}