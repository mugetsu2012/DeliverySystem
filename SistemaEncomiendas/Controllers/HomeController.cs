using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaEncomiendas.Core.Providers;

namespace SistemaEncomiendas.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICifradoProvider _cifradoProvider;
        private readonly ILogger _logger;

        public HomeController(ICifradoProvider cifradoProvider, ILogger<HomeController> logger)
        {
            _cifradoProvider = cifradoProvider;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Cliente"))
            {
                return RedirectToAction("VerExcel", "Clientes");
            }
            else if (User.IsInRole("Vendedor"))
            {
                return RedirectToAction("VerExcel", "Vendedores");
            }
            else
            {
                return RedirectToAction("Paquetes", "Administracion");
            }
        }
    }
}