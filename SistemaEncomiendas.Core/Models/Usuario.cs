using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemaEncomiendas.Core.Models
{
    public class Usuario
    {
        public Usuario()
        {
            FechaIngreso = DateTime.Now;
            Activo = true;
        }

        public string NombreUsuario { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        /// <summary>
        /// Indica que tipo de usuario es
        /// </summary>
        public TipoUsuario TipoUsuario { get; set; }

        /// <summary>
        /// Password encriptada del usuario
        /// </summary>
        public byte[] PassWord { get; set; }

        /// <summary>
        /// Indica si el usuario está activo o no
        /// </summary>
        public bool Activo { get; set; }

        public DateTime FechaIngreso { get; set; }

        public List<Paquete> PaquetesEnvia { get; set; }

        public List<Paquete> PaquetesRecibe { get; set; }

        public string ImprimirNombreCompleto()
        {
            return Nombre + " " + Apellido;
        }

        public string ImprimirNombreConUsuario()
        {
            return Nombre + " " + Apellido + " ("+ NombreUsuario +")";
        }

        public string ImprimirNombreCompletoConUsuarioYTipo()
        {
            return Nombre + " " + Apellido + " (" + NombreUsuario + ")" + "-" + TipoUsuario;
        }

    }
}
