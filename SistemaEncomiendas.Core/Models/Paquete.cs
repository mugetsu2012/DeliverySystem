using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEncomiendas.Core.Models
{
    public class Paquete
    {
        public Paquete()
        {
            FechaIngreso = DateTime.Now;
        }

        public int Codigo { get; set; }

        /// <summary>
        /// Id del usuario que envia
        /// </summary>
        public string IdUsuarioEnvia { get; set; }

        /// <summary>
        /// Id del usuario que recibe
        /// </summary>
        public string IdUsuarioRecibe { get; set; }

        /// <summary>
        /// Precio designado para este paquete
        /// </summary>
        public decimal Precio { get; set; }

        /// <summary>
        /// Lugar en el que se pacta entregar el paquete
        /// </summary>
        public string LugarEntrega { get; set; }

        /// <summary>
        /// Fecha de ingreso al sistema
        /// </summary>
        public DateTime FechaIngreso { get; set; }

        /// <summary>
        /// Indica la fecha en la que debe entregarse el paquete
        /// </summary>
        public DateTime FechaEntrega { get; set; }

        /// <summary>
        /// Estado actual del paquete
        /// </summary>
        public EstadoPaquete EstadoPaquete { get; set; }

        /// <summary>
        /// Fecha en la cual el paquete pasa a Entregado o Anulado
        /// </summary>
        public DateTime? FechaCambioEstado { get; set; }

        /// <summary>
        /// Fecha en la que se dijo que el paquete fue entregado
        /// </summary>
        public DateTime? FechaPaqueteEntregado { get; set; }

        /// <summary>
        /// Fecha en la que se dijo que el paquete fue anulado
        /// </summary>
        public DateTime? FechaPaqueteAnulado { get; set; }

        /// <summary>
        /// Comentario opcional al momento de crear el paquete
        /// </summary>
        public string Comentario { get; set; }

        public Usuario UsuarioEnvia { get; set; }

        public Usuario UsuarioRecibe { get; set; }
    }
}
