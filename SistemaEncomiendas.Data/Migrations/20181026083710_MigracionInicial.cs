using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaEncomiendas.Data.Migrations
{
    public partial class MigracionInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    NombreUsuario = table.Column<string>(maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Apellido = table.Column<string>(maxLength: 200, nullable: false),
                    TipoUsuario = table.Column<int>(nullable: false),
                    PassWord = table.Column<byte[]>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    FechaIngreso = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.NombreUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Paquete",
                columns: table => new
                {
                    Codigo = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdUsuarioEnvia = table.Column<string>(maxLength: 100, nullable: false),
                    IdUsuarioRecibe = table.Column<string>(maxLength: 100, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    LugarEntrega = table.Column<string>(maxLength: 5000, nullable: false),
                    FechaIngreso = table.Column<DateTime>(nullable: false),
                    FechaEntrega = table.Column<DateTime>(nullable: false),
                    EstadoPaquete = table.Column<int>(nullable: false),
                    FechaCambioEstado = table.Column<DateTime>(nullable: true),
                    Comentario = table.Column<string>(maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paquete", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Paquete_Usuario_IdUsuarioEnvia",
                        column: x => x.IdUsuarioEnvia,
                        principalTable: "Usuario",
                        principalColumn: "NombreUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Paquete_Usuario_IdUsuarioRecibe",
                        column: x => x.IdUsuarioRecibe,
                        principalTable: "Usuario",
                        principalColumn: "NombreUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Paquete_IdUsuarioEnvia",
                table: "Paquete",
                column: "IdUsuarioEnvia");

            migrationBuilder.CreateIndex(
                name: "IX_Paquete_IdUsuarioRecibe",
                table: "Paquete",
                column: "IdUsuarioRecibe");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paquete");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
