using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Data.Configurations
{
    public class PaqueteConfig: IEntityTypeConfiguration<Paquete>
    {
        public void Configure(EntityTypeBuilder<Paquete> builder)
        {
            builder.ToTable(nameof(Paquete));

            builder.HasKey(x => x.Codigo);

            builder.Property(x => x.Codigo).UseSqlServerIdentityColumn();

            builder.Property(x => x.IdUsuarioEnvia).IsRequired().HasMaxLength(100);

            builder.Property(x => x.IdUsuarioRecibe).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Precio).IsRequired().HasColumnType("decimal(18,6)");

            builder.Property(x => x.LugarEntrega).IsRequired().HasMaxLength(5000);

            builder.Property(x => x.FechaIngreso).IsRequired();

            builder.Property(x => x.FechaEntrega).IsRequired();

            builder.Property(x => x.EstadoPaquete).IsRequired();

            builder.Property(x => x.Comentario).HasMaxLength(5000);

            //builder.HasOne(x => x.UsuarioEnvia).WithMany(m => m.PaquetesEnvia).HasForeignKey(f => f.IdUsuarioEnvia)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(x => x.UsuarioRecibe).WithMany(m => m.PaquetesRecibe).HasForeignKey(f => f.IdUsuarioRecibe)
            //    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
