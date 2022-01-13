using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Models
{
    public class ListaPaquetesVendedoresVm:ListaPaquetesVm
    {
        public List<Usuario> Clientes { get; set; }

        public string IdCliente { get; set; }

        public Usuario Vendedor { get; set; }
    }
}
