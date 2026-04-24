using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaCursos.Migrations
{
    /// <inheritdoc />
    public partial class AddMatriculasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matricula_AspNetUsers_UserId",
                table: "Matricula");

            migrationBuilder.DropForeignKey(
                name: "FK_Matricula_Cursos_CursoId",
                table: "Matricula");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Matricula",
                table: "Matricula");

            migrationBuilder.RenameTable(
                name: "Matricula",
                newName: "Matriculas");

            migrationBuilder.RenameIndex(
                name: "IX_Matricula_UserId",
                table: "Matriculas",
                newName: "IX_Matriculas_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Matricula_CursoId",
                table: "Matriculas",
                newName: "IX_Matriculas_CursoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Matriculas",
                table: "Matriculas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matriculas_AspNetUsers_UserId",
                table: "Matriculas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matriculas_Cursos_CursoId",
                table: "Matriculas",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matriculas_AspNetUsers_UserId",
                table: "Matriculas");

            migrationBuilder.DropForeignKey(
                name: "FK_Matriculas_Cursos_CursoId",
                table: "Matriculas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Matriculas",
                table: "Matriculas");

            migrationBuilder.RenameTable(
                name: "Matriculas",
                newName: "Matricula");

            migrationBuilder.RenameIndex(
                name: "IX_Matriculas_UserId",
                table: "Matricula",
                newName: "IX_Matricula_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Matriculas_CursoId",
                table: "Matricula",
                newName: "IX_Matricula_CursoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Matricula",
                table: "Matricula",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matricula_AspNetUsers_UserId",
                table: "Matricula",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matricula_Cursos_CursoId",
                table: "Matricula",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
