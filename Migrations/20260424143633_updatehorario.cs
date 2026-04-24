using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaCursos.Migrations
{
    /// <inheritdoc />
    public partial class updatehorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Curso_Horario",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "HorarioFin",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "HorarioInicio",
                table: "Cursos");

            migrationBuilder.AddColumn<int>(
                name: "HoraFin",
                table: "Cursos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HoraInicio",
                table: "Cursos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Curso_Horario",
                table: "Cursos",
                sql: "HoraInicio < HoraFin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Curso_Horario",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "HoraFin",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "Cursos");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HorarioFin",
                table: "Cursos",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HorarioInicio",
                table: "Cursos",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddCheckConstraint(
                name: "CK_Curso_Horario",
                table: "Cursos",
                sql: "HorarioInicio < HorarioFin");
        }
    }
}
