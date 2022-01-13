using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaEncomiendas.Data.Migrations
{
    public partial class Multimedias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MultimediaUsuario",
                columns: table => new
                {
                    Codigo = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdUsuario = table.Column<string>(maxLength: 100, nullable: false),
                    NombreArchivo = table.Column<string>(maxLength: 200, nullable: false),
                    MimeType = table.Column<string>(maxLength: 200, nullable: false),
                    TipoArchivo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultimediaUsuario", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_MultimediaUsuario_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "NombreUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultimediaUsuario_IdUsuario",
                table: "MultimediaUsuario",
                column: "IdUsuario");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultimediaUsuario");
        }
    }
}
