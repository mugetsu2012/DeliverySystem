using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SistemaEncomiendas.Core.Data.Repositories;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Data.Repositories
{
    public class PaqueteRepository: GenericRepository<Paquete>, IPaqueteRepository
    {
        private readonly EncomiendasContext _dbContext;

        public PaqueteRepository(DbContext context) : base(context)
        {
            _dbContext = context as EncomiendasContext;
        }
    }
}
