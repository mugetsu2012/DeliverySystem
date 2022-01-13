using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Models
{
    public class AsignarExcelUsuarioVm
    {
        [Required(ErrorMessage = "Debe seleccionar el usuario")]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = "El archivo Excel es requerido")]
        public IFormFile Archivo { get; set; }

        public List<IFormFile> Imagenes { get; set; }

        public List<Usuario> Usuarios { get; set; }
    }
}
