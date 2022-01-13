using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Models
{
    public class ListaPaquetesVm
    {
        public ListaPaquetesVm()
        {
            Paquetes = new List<Paquete>();
        }
        public List<Paquete> Paquetes { get; set; }

        public string FechaInicioEntrega { get; set; }

        public string FechaFinEntrega { get; set; }

        public string LugarEntrega { get; set; }

        public EstadoPaquete? EstadoPaquete { get; set; }
    }
}
