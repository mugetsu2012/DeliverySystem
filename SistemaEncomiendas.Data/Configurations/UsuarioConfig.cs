using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Data.Configurations
{
    public class UsuarioConfig:IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable(nameof(Usuario));

            builder.HasKey(x => x.NombreUsuario);

            builder.Property(x => x.NombreUsuario).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Nombre).IsRequired().HasMaxLength(200);

            builder.Property(x => x.Apellido).IsRequired().HasMaxLength(200);

            builder.Property(x => x.TipoUsuario).IsRequired();

            builder.Property(x => x.PassWord).IsRequired();

            builder.Property(x => x.Activo).IsRequired();

            builder.Property(x => x.FechaIngreso).IsRequired();

            builder.HasMany(x => x.PaquetesEnvia).WithOne(y => y.UsuarioEnvia).HasForeignKey(y => y.IdUsuarioEnvia)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasMany(x => x.PaquetesRecibe).WithOne(y => y.UsuarioRecibe).HasForeignKey(y => y.IdUsuarioRecibe)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
