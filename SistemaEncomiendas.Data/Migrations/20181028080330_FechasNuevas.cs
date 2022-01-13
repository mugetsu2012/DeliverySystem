using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaEncomiendas.Data.Migrations
{
    public partial class FechasNuevas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPaqueteAnulado",
                table: "Paquete",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPaqueteEntregado",
                table: "Paquete",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaPaqueteAnulado",
                table: "Paquete");

            migrationBuilder.DropColumn(
                name: "FechaPaqueteEntregado",
                table: "Paquete");
        }
    }
}
