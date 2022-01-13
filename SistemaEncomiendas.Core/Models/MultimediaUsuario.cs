using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEncomiendas.Core.Models
{
    public class MultimediaUsuario
    {
        public int Codigo { get; set; }

        public string IdUsuario { get; set; }

        public string NombreArchivo { get; set; }

        public string MimeType { get; set; }

        public TipoArchivo TipoArchivo { get; set; }

        public Usuario Usuario { get; set; }
    }
}
