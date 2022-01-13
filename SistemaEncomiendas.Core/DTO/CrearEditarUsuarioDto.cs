using System;
using System.Collections.Generic;
using System.Text;
using SistemaEncomiendas.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaEncomiendas.Core.DTO
{
    /// <summary>
    /// DTO para la creacione/edicion de usuarios
    /// </summary>
    public class CrearEditarUsuarioDto
    {
        /// <summary>
        /// Variable que me ayuda a saber si el usuario es nuevo o no
        /// </summary>
        public bool EsNuevo { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(100, ErrorMessage = "El usuario no puede tener mas de 100 caracteres")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener mas de 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede tener mas de 100 caracteres")]
        public string Apellido { get; set; }

        /// <summary>
        /// Indica que tipo de usuario es
        /// </summary>
        public TipoUsuario TipoUsuario { get; set; }

        /// <summary>
        /// Password en string, luego se guarda cifrada
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña no puede tener mas de 100 caracteres")]
        public string PassWordTexto { get; set; }
    }
}
