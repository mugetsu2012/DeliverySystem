using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaEncomiendas.Core.DTO;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Models
{
    public class CrearEditarPaqueteVm
    {
        public CrearEditarPaqueteDto CrearEditarPaqueteDto { get; set; }

        public List<Usuario> Vendedores { get; set; }

        public List<Usuario> Clientes { get; set; }
    }
}
