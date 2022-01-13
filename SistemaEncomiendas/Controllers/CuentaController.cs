using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaEncomiendas.Core.Models;
using SistemaEncomiendas.Core.Services;
using SistemaEncomiendas.Models;

namespace SistemaEncomiendas.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IUsuariosService _usuariosService;

        public CuentaController(IUsuariosService usuariosService)
        {
            _usuariosService = usuariosService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginVm model = new LoginVm();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ValidarLogin(LoginVm loginVm)
        {
            if (!ModelState.IsValid)
            {
                List<string> errores = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();

                return Json(new {exito = false, errores});
            }

            bool loginValido = await _usuariosService.ValidarLoginAsync(loginVm.Usuario, loginVm.Password);

            if (loginValido)
            {
                var usuario = await _usuariosService.GetUsuarioAsync(loginVm.Usuario);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginVm.Usuario));
                identity.AddClaim(new Claim(ClaimTypes.Name, loginVm.Usuario));
                identity.AddClaim(new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString()));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new Microsoft.AspNetCore.Authentication.AuthenticationProperties { IsPersistent = true });

            }

            return Json(new {exito = true, loginValido, urlNew = Url.Action("RedireccionarUsuario", "Cuenta")});
        }

        [Authorize]
        public async Task<IActionResult> RedireccionarUsuario()
        {
            
            if (User.IsInRole(TipoUsuario.Admin.ToString()))
            {
                //Como es admin, lo mando al manejo de usuarios
                return RedirectToAction("Usuarios", "Administracion");
            }
            else if (User.IsInRole(TipoUsuario.Vendedor.ToString()))
            {
                //Si es vendedor lo mando a sus ventas
                return RedirectToAction("Index", "Home");
            }
            else if (User.IsInRole(TipoUsuario.Cliente.ToString()))
            {
                //Si es cliente lo mando a sus compras
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Si entra aca es porque esta logueado pero no reconozco su rol
                //Asi que lo deslogueo y lo mando al login
                await HttpContext.SignOutAsync();
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult PermisoDenegado()
        {
            return View();
        }
    }
}