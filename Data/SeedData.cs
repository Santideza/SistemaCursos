using Microsoft.AspNetCore.Identity;
using SistemaCursos.Models;

namespace SistemaCursos.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var context     = services.GetRequiredService<ApplicationDbContext>();

        foreach (var rol in new[] { "Coordinador", "Estudiante" })
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        await CrearUsuario(userManager,
            email: "coordinador@demo.com",
            password: "Coord@1234",
            rol: "Coordinador",
            nombre: "Juan",
            apellido: "Pérez"
        );

        await CrearUsuario(userManager,
            email: "estudiante@demo.com",
            password: "Estud@1234",
            rol: "Estudiante",
            nombre: "María",
            apellido: "Gómez"
        );

        if (!context.Cursos.Any())
        {
            context.Cursos.AddRange(
                new Curso {
                Codigo = "CS101",
                Nombre = "Introducción a la Programación",
                Creditos = 3,
                CupoMaximo = 30,
                HoraInicio = 9,
                HoraFin = 11,
                Activo = true
                },
                new Curso {
                    Codigo = "BD201",
                    Nombre = "Bases de Datos",
                    Creditos = 4,
                    CupoMaximo = 25,
                    HoraInicio = 11,
                    HoraFin = 13,
                    Activo = true
                },
                new Curso {
                    Codigo = "WEB301",
                    Nombre = "Desarrollo Web",
                    Creditos = 3,
                    CupoMaximo = 28,
                    HoraInicio = 14,
                    HoraFin = 16,
                    Activo = true
                }
            );

            await context.SaveChangesAsync();
        }
    }

    private static async Task CrearUsuario(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string rol,
        string nombre,
        string apellido)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, rol);

                await userManager.AddClaimAsync(user,
                    new System.Security.Claims.Claim("Nombre", $"{nombre} {apellido}"));
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(user, rol))
                await userManager.AddToRoleAsync(user, rol);
        }
    }
}