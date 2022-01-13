﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SistemaEncomiendas.Data;

namespace SistemaEncomiendas.Data.Migrations
{
    [DbContext(typeof(EncomiendasContext))]
    [Migration("20190210072535_Multimedias")]
    partial class Multimedias
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SistemaEncomiendas.Core.Models.MultimediaUsuario", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("IdUsuario")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("NombreArchivo")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("TipoArchivo");

                    b.HasKey("Codigo");

                    b.HasIndex("IdUsuario");

                    b.ToTable("MultimediaUsuario");
                });

            modelBuilder.Entity("SistemaEncomiendas.Core.Models.Paquete", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comentario")
                        .HasMaxLength(5000);

                    b.Property<int>("EstadoPaquete");

                    b.Property<DateTime?>("FechaCambioEstado");

                    b.Property<DateTime>("FechaEntrega");

                    b.Property<DateTime>("FechaIngreso");

                    b.Property<DateTime?>("FechaPaqueteAnulado");

                    b.Property<DateTime?>("FechaPaqueteEntregado");

                    b.Property<string>("IdUsuarioEnvia")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("IdUsuarioRecibe")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("LugarEntrega")
                        .IsRequired()
                        .HasMaxLength(5000);

                    b.Property<decimal>("Precio")
                        .HasColumnType("decimal(18,6)");

                    b.HasKey("Codigo");

                    b.HasIndex("IdUsuarioEnvia");

                    b.HasIndex("IdUsuarioRecibe");

                    b.ToTable("Paquete");
                });

            modelBuilder.Entity("SistemaEncomiendas.Core.Models.Usuario", b =>
                {
                    b.Property<string>("NombreUsuario")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.Property<bool>("Activo");

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime>("FechaIngreso");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<byte[]>("PassWord")
                        .IsRequired();

                    b.Property<int>("TipoUsuario");

                    b.HasKey("NombreUsuario");

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("SistemaEncomiendas.Core.Models.MultimediaUsuario", b =>
                {
                    b.HasOne("SistemaEncomiendas.Core.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SistemaEncomiendas.Core.Models.Paquete", b =>
                {
                    b.HasOne("SistemaEncomiendas.Core.Models.Usuario", "UsuarioEnvia")
                        .WithMany("PaquetesEnvia")
                        .HasForeignKey("IdUsuarioEnvia");

                    b.HasOne("SistemaEncomiendas.Core.Models.Usuario", "UsuarioRecibe")
                        .WithMany("PaquetesRecibe")
                        .HasForeignKey("IdUsuarioRecibe");
                });
#pragma warning restore 612, 618
        }
    }
}