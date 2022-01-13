using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Models
{
    public class VerExcelVm
    {
        public List<MultimediaUsuario> MultimediaUsuarios { get; set; }

        public string BaseUrl { get; set; }
    }
}
