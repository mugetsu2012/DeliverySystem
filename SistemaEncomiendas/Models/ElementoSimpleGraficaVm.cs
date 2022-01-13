using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaEncomiendas.Models
{
    /// <summary>
    /// Elemento que ayuda a representar graficas
    /// </summary>
    public class ElementoSimpleGraficaVm
    {
        /// <summary>
        /// Texto que sale en la grafica
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Valor a mostrar en la grafica
        /// </summary>
        public decimal Valor { get; set; }
    }
}
