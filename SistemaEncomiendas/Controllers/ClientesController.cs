using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SelectPdf;
using SistemaEncomiendas.Core.Models;
using SistemaEncomiendas.Core.Services;
using SistemaEncomiendas.Helpers;
using SistemaEncomiendas.Models;

namespace SistemaEncomiendas.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IPaqueteService _paqueteService;
        private readonly IUsuariosService _usuariosService;
        private readonly IViewRenderService _viewRenderService;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public ClientesController(IPaqueteService paqueteService,
            IViewRenderService viewRenderService, ILogger<ClientesController> logger,
            IUsuariosService usuariosService,
            IConfiguration configuration)
        {
            _paqueteService = paqueteService;
            _viewRenderService = viewRenderService;
            _logger = logger;
            _usuariosService = usuariosService;
            _configuration = configuration;
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Index(ListaPaquetesUsuarioVm listaPaquetesUsuarioVm)
        {
            DateTime fechaIniFiltro;
            DateTime fechaFinFiltro;

            if (string.IsNullOrEmpty(listaPaquetesUsuarioVm.FechaInicioEntrega))
            {
                fechaIniFiltro = DateTime.Now.AddMonths(-3);
                listaPaquetesUsuarioVm.FechaInicioEntrega = DateTime.Now.AddMonths(-3).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fechaIniFiltro = DateTime.ParseExact(
                    listaPaquetesUsuarioVm.FechaInicioEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(listaPaquetesUsuarioVm.FechaFinEntrega))
            {
                fechaFinFiltro = DateTime.Now.AddMonths(3);
                listaPaquetesUsuarioVm.FechaFinEntrega = DateTime.Now.AddMonths(3).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fechaFinFiltro = DateTime.ParseExact(
                    listaPaquetesUsuarioVm.FechaFinEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);
            }

            listaPaquetesUsuarioVm.Paquetes =
                await _paqueteService.GetPaquetesCliente(fechaIniFiltro, fechaFinFiltro, User.Identity.Name,
                    listaPaquetesUsuarioVm.LugarEntrega, listaPaquetesUsuarioVm.EstadoPaquete);

            return View(listaPaquetesUsuarioVm);
        }

        [HttpGet]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> GenerarReporte(ListaPaquetesUsuarioVm listaPaquetesUsuarioVm)
        {
            DateTime fechaIniFiltro;
            DateTime fechaFinFiltro;
            _logger.LogWarning("Entra al controller de Clientes, la data es: " + Newtonsoft.Json.JsonConvert.SerializeObject(listaPaquetesUsuarioVm));
            //Debemos hacer una transformacion para evitar problemas de problemas
            listaPaquetesUsuarioVm.FechaInicioEntrega = listaPaquetesUsuarioVm.FechaInicioEntrega.Replace("p. m.", "PM");
            listaPaquetesUsuarioVm.FechaInicioEntrega = listaPaquetesUsuarioVm.FechaInicioEntrega.Replace("a. m.", "AM");

            listaPaquetesUsuarioVm.FechaFinEntrega = listaPaquetesUsuarioVm.FechaFinEntrega.Replace("a. m.", "AM");
            listaPaquetesUsuarioVm.FechaFinEntrega = listaPaquetesUsuarioVm.FechaFinEntrega.Replace("p. m.", "PM");

            if (string.IsNullOrEmpty(listaPaquetesUsuarioVm.FechaInicioEntrega))
            {
                fechaIniFiltro = DateTime.Now.AddMonths(-3);
                listaPaquetesUsuarioVm.FechaInicioEntrega = DateTime.Now.AddMonths(-3).ToString("d/M/yyyy");
            }
            else
            {
                fechaIniFiltro = DateTime.ParseExact(
                    listaPaquetesUsuarioVm.FechaInicioEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(listaPaquetesUsuarioVm.FechaFinEntrega))
            {
                fechaFinFiltro = DateTime.Now.AddMonths(3);
                listaPaquetesUsuarioVm.FechaFinEntrega = DateTime.Now.AddMonths(3).ToString("d/M/yyyy");
            }
            else
            {
                fechaFinFiltro = DateTime.ParseExact(
                    listaPaquetesUsuarioVm.FechaFinEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);
            }

            listaPaquetesUsuarioVm.Paquetes =
                await _paqueteService.GetPaquetesCliente(fechaIniFiltro, fechaFinFiltro, User.Identity.Name,
                    listaPaquetesUsuarioVm.LugarEntrega, listaPaquetesUsuarioVm.EstadoPaquete);

            //return View("_ReporteClientes", listaPaquetesUsuarioVm.Paquetes);
            Usuario cliente = await _usuariosService.GetUsuarioAsync(User.Identity.Name);
            listaPaquetesUsuarioVm.Cliente = cliente;

            string viewString = await _viewRenderService.RenderToStringAsync("Clientes/_ReporteClientes", listaPaquetesUsuarioVm);

            _logger.LogWarning("Ya estamos por mandar a generar el PDF");
            HtmlToPdf converter = new HtmlToPdf();

            Stream stream = new MemoryStream();
            PdfDocument doc = converter.ConvertHtmlString(viewString);

            doc.Save(stream);
            stream.Position = 0;
            doc.Close();
            _logger.LogWarning("Al parecer hemos llegado al punto donde ya se puede regresar el archivo");

            return File(stream, "application/octet-stream","Reporte.pdf");

        }

        [HttpGet]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> VerExcel()
        {
            //Sacamos las multimedias de este usuario
            List<MultimediaUsuario> multimediaUsuarios = await _usuariosService.MultimediasUsuario(User.Identity.Name);
            VerExcelVm model = new VerExcelVm()
            {
                MultimediaUsuarios = multimediaUsuarios,
                BaseUrl = _configuration["BaseUrlStorage"]
            };

            return View(model);
        }

    }
}