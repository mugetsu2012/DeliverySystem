using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaEncomiendas.Models
{
    public class EstadisticasVm
    {
        public int PaquetesPendientes { get; set; }

        public int PaquetesAnulados { get; set; }

        public int PaquetesEntregados { get; set; }

        public int CantidadClientes { get; set; }
    }
}
