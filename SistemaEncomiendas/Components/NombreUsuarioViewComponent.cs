using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SistemaEncomiendas.Core.Models;
using SistemaEncomiendas.Core.Services;

namespace SistemaEncomiendas.Components
{
    public class NombreUsuarioViewComponent: ViewComponent
    {
        private readonly IUsuariosService _usuariosService;

        public NombreUsuarioViewComponent(IUsuariosService usuariosService)
        {
            _usuariosService = usuariosService;
        }

        public async Task<string> InvokeAsync()
        {
            Usuario usuario = await _usuariosService.GetUsuarioAsync(User.Identity.Name);

            return usuario.ImprimirNombreCompleto();
        }
    }
}
