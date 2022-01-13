using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using SistemaEncomiendas.Core.Models;
using SistemaEncomiendas.Core.Services;
using SistemaEncomiendas.Helpers;
using SistemaEncomiendas.Models;

namespace SistemaEncomiendas.Controllers
{
    [Authorize]
    public class VendedoresController : Controller
    {
        private readonly IPaqueteService _paqueteService;
        private readonly IUsuariosService _usuariosService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IConfiguration _configuration;

        public VendedoresController(IPaqueteService paqueteService,
            IUsuariosService usuariosService, 
            IViewRenderService viewRenderService, 
            IConfiguration configuration)
        {
            _paqueteService = paqueteService;
            _usuariosService = usuariosService;
            _viewRenderService = viewRenderService;
            _configuration = configuration;
        }


        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Index(ListaPaquetesVendedoresVm listaPaquetesVendedoresVm)
        {
            DateTime fechaIniFiltro;
            DateTime fechaFinFiltro;
            string idCliente = listaPaquetesVendedoresVm.IdCliente == "TODOS"
                ? ""
                : listaPaquetesVendedoresVm.IdCliente;

            if (string.IsNullOrEmpty(listaPaquetesVendedoresVm.FechaInicioEntrega))
            {
                fechaIniFiltro = DateTime.Now.AddMonths(-3);
                listaPaquetesVendedoresVm.FechaInicioEntrega = DateTime.Now.AddMonths(-3).ToString("d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fechaIniFiltro = DateTime.ParseExact(
                    listaPaquetesVendedoresVm.FechaInicioEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(listaPaquetesVendedoresVm.FechaFinEntrega))
            {
                fechaFinFiltro = DateTime.Now.AddMonths(3);
                listaPaquetesVendedoresVm.FechaFinEntrega = DateTime.Now.AddMonths(3).ToString("d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fechaFinFiltro = DateTime.ParseExact(
                    listaPaquetesVendedoresVm.FechaFinEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);
            }

            listaPaquetesVendedoresVm.Paquetes =
                await _paqueteService.GetPaquetesVendedor(fechaIniFiltro, fechaFinFiltro, User.Identity.Name,
                    listaPaquetesVendedoresVm.LugarEntrega, listaPaquetesVendedoresVm.EstadoPaquete, idCliente);

            listaPaquetesVendedoresVm.Clientes = await _usuariosService.GetClientesVendedor(User.Identity.Name);

            return View(listaPaquetesVendedoresVm);
        }

        [HttpGet]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> GenerarReporte(ListaPaquetesVendedoresVm listaPaquetesVendedoresVm)
        {
            DateTime fechaIniFiltro;
            DateTime fechaFinFiltro;

            string idCliente = listaPaquetesVendedoresVm.IdCliente == "TODOS"
                ? ""
                : listaPaquetesVendedoresVm.IdCliente;

            //Debemos hacer una transformacion para evitar problemas de problemas
            listaPaquetesVendedoresVm.FechaInicioEntrega = listaPaquetesVendedoresVm.FechaInicioEntrega.Replace("p. m.", "PM");
            listaPaquetesVendedoresVm.FechaInicioEntrega = listaPaquetesVendedoresVm.FechaInicioEntrega.Replace("a. m.", "AM");

            listaPaquetesVendedoresVm.FechaFinEntrega = listaPaquetesVendedoresVm.FechaFinEntrega.Replace("a. m.", "AM");
            listaPaquetesVendedoresVm.FechaFinEntrega = listaPaquetesVendedoresVm.FechaFinEntrega.Replace("p. m.", "PM");

            if (string.IsNullOrEmpty(listaPaquetesVendedoresVm.FechaInicioEntrega))
            {
                fechaIniFiltro = DateTime.Now.AddMonths(-3);
                listaPaquetesVendedoresVm.FechaInicioEntrega = DateTime.Now.AddMonths(-3).ToString("d/M/yyyy");
            }
            else
            {
                fechaIniFiltro = DateTime.ParseExact(
                    listaPaquetesVendedoresVm.FechaInicioEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrEmpty(listaPaquetesVendedoresVm.FechaFinEntrega))
            {
                fechaFinFiltro = DateTime.Now.AddMonths(3);
                listaPaquetesVendedoresVm.FechaFinEntrega = DateTime.Now.AddMonths(3).ToString("d/M/yyyy");
            }
            else
            {
                fechaFinFiltro = DateTime.ParseExact(
                    listaPaquetesVendedoresVm.FechaFinEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);
            }

            listaPaquetesVendedoresVm.Paquetes =
                await _paqueteService.GetPaquetesVendedor(fechaIniFiltro, fechaFinFiltro, User.Identity.Name,
                    listaPaquetesVendedoresVm.LugarEntrega, listaPaquetesVendedoresVm.EstadoPaquete, idCliente);

            listaPaquetesVendedoresVm.Clientes = await _usuariosService.GetClientesVendedor(User.Identity.Name);

            Usuario vendedor = await _usuariosService.GetUsuarioAsync(User.Identity.Name);
            listaPaquetesVendedoresVm.Vendedor = vendedor;

            //return View("_ReporteVendedores", listaPaquetesVendedoresVm);
           

            string viewString = await _viewRenderService.RenderToStringAsync("Vendedores/_ReporteVendedores", listaPaquetesVendedoresVm);

            HtmlToPdf converter = new HtmlToPdf();

            Stream stream = new MemoryStream();
            PdfDocument doc = converter.ConvertHtmlString(viewString);

            doc.Save(stream);
            stream.Position = 0;
            doc.Close();
            return File(stream, "application/octet-stream", "Reporte.pdf");

        }

        [HttpGet]
        [Authorize(Roles = "Vendedor")]
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