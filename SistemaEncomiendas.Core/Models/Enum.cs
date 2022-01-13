using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEncomiendas.Core.Models
{
    public enum TipoUsuario
    {
        Admin,
        Vendedor,
        Cliente
    }

    public enum EstadoPaquete
    {
        Pendiente,
        Entregado,
        Anulado
    }

    public enum TipoArchivo
    {
        Excel,
        Imagen
    }
}
