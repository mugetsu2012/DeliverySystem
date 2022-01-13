using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Data.Configurations
{
    class MultimediaUsuarioConfig:IEntityTypeConfiguration<MultimediaUsuario>
    {
        public void Configure(EntityTypeBuilder<MultimediaUsuario> builder)
        {
            builder.ToTable(nameof(MultimediaUsuario));

            builder.HasKey(x => x.Codigo);

            builder.Property(x => x.IdUsuario).IsRequired().HasMaxLength(100);
            builder.Property(x => x.MimeType).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TipoArchivo).IsRequired();
            builder.Property(x => x.NombreArchivo).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Usuario).WithMany().HasForeignKey(f => f.IdUsuario);
        }
    }
}
