using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SistemaEncomiendas.Core.DTO
{
    public class CrearEditarPaqueteDto
    {
        public int Codigo { get; set; }

        /// <summary>
        /// Id del usuario que envia
        /// </summary>
        [Required(ErrorMessage = "El usuario que envia es requerido")]
        public string IdUsuarioEnvia { get; set; }

        /// <summary>
        /// Id del usuario que recibe
        /// </summary>
        [Required(ErrorMessage = "El usuario que recibe es requerido")]
        public string IdUsuarioRecibe { get; set; }

        /// <summary>
        /// Precio designado para este paquete
        /// </summary>
        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.0,999999,ErrorMessage = "El precio debe ser mayor a cero")]
        public decimal Precio { get; set; }

        /// <summary>
        /// Lugar en el que se pacta entregar el paquete
        /// </summary>
        [Required(ErrorMessage = "El lugar de entrega es requerido")]
        [MaxLength(5000, ErrorMessage = "El lugar de entrega no puede tener mas de 5000 caracteres")]
        public string LugarEntrega { get; set; }

        /// <summary>
        /// Indica la fecha en la que debe entregarse el paquete
        /// </summary>
        public DateTime FechaEntrega { get; set; }

        /// <summary>
        /// Comentario opcional al momento de crear el paquete
        /// </summary>
        [MaxLength(5000, ErrorMessage = "El comentario no puede tener mas de 5000 caracteres")]
        public string Comentario { get; set; }

        /// <summary>
        /// Propiedad usada para recibir en texto la fecha
        /// </summary>
        [Required(ErrorMessage = "La fecha de entrega es requerida")]
        public string FechaEntregaTexto { get; set; }
    }
}
