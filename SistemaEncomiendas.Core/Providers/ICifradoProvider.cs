using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaEncomiendas.Core.Providers
{
    public interface ICifradoProvider
    {
        /// <summary>
        /// Se encarga de cifrar un string a byte Array
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        byte[] CifrarTexto(string texto);
    }
}
