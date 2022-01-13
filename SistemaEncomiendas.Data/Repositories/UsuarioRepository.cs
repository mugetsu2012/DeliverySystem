using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SistemaEncomiendas.Core.Data;
using SistemaEncomiendas.Core.Data.Repositories;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Data.Repositories
{
    public class UsuarioRepository: GenericRepository<Usuario>, IUsuarioRepository
    {
        private EncomiendasContext _dbContext;

        public UsuarioRepository(DbContext context) : base(context)
        {
            _dbContext = context as EncomiendasContext;
        }
    }
}
