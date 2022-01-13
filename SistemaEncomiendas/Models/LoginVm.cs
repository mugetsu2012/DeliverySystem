using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaEncomiendas.Models
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Debes ingresar un usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Debes ingresar una contraseña")]
        public string Password { get; set; }
    }
}
