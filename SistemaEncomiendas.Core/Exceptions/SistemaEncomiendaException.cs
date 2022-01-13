using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEncomiendas.Core.Exceptions
{
    public class SistemaEncomiendaException: Exception
    {
        public SistemaEncomiendaException(string mensaje): base(mensaje)
        {
            
        }
    }
}
