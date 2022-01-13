using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Models
{
    public class ListaUsuariosVm
    {
        public ListaUsuariosVm()
        {
            IncluirInactivos = false;
            Usuarios = new List<Usuario>();
        }

        public List<Usuario> Usuarios { get; set; }

        public string Nombre { get; set; }

        public TipoUsuario? TipoUsuario { get; set; }

        public bool IncluirInactivos { get; set; }
    }
}
