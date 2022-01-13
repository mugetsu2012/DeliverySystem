using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Models
{
    public class ListaPaquetesUsuarioVm:ListaPaquetesVm
    {
        //TODO: Esto queda tentativo, de momento no se utilizara
        public List<Usuario> UsuariosVendedores { get; set; }

        public string IdUsuarioVendedor { get; set; }

        public Usuario Cliente { get; set; }
    }
}
