using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SistemaEncomiendas.Core.Data.Repositories;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Data.Repositories
{
    public class MultimediaUsuarioRepository: GenericRepository<MultimediaUsuario>, IMultimediaUsuarioRepository
    {
        private readonly EncomiendasContext _dbContext;

        public MultimediaUsuarioRepository(DbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext as EncomiendasContext;
        }
    }
}
