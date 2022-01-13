using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SistemaEncomiendas.Core.Models;
using SistemaEncomiendas.Data.Configurations;

namespace SistemaEncomiendas.Data
{
    public class EncomiendasContext: DbContext
    {
        public EncomiendasContext(DbContextOptions<EncomiendasContext> options):base(options)
        {
            
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Paquete> Paquetes { get; set; }

        public DbSet<MultimediaUsuario> MultimediaUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UsuarioConfig());

            modelBuilder.ApplyConfiguration(new PaqueteConfig());

            modelBuilder.ApplyConfiguration(new MultimediaUsuarioConfig());
        }
    }
}
