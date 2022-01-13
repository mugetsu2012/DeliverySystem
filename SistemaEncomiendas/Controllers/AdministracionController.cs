using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using SelectPdf;
using SistemaEncomiendas.Core.DTO;
using SistemaEncomiendas.Core.Exceptions;
using SistemaEncomiendas.Core.Models;
using SistemaEncomiendas.Core.Services;
using SistemaEncomiendas.Helpers;
using SistemaEncomiendas.Models;

namespace SistemaEncomiendas.Controllers
{
    [Authorize]
    public class AdministracionController : Controller
    {
        private readonly IUsuariosService _usuariosService;
        private readonly IPaqueteService _paqueteService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IViewRenderService _viewRenderService;
        private readonly IConfiguration _configuration;

        public AdministracionController(IUsuariosService usuariosService, 
            IMapper mapper,
            IPaqueteService paqueteService, ILogger<AdministracionController> logger, 
            IViewRenderService viewRenderService, 
            IConfiguration configuration)
        {
            _usuariosService = usuariosService;
            _mapper = mapper;
            _paqueteService = paqueteService;
            _logger = logger;
            _viewRenderService = viewRenderService;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Usuarios(string nombre="", bool incluirInactivos = false, TipoUsuario? tipoUsuario = null)
        {
            List<Usuario> usuarios =
                await _usuariosService.GetListaUsuariosAsync(nombre, tipoUsuario, incluirInactivos);

            ListaUsuariosVm model = new ListaUsuariosVm()
            {
                TipoUsuario = tipoUsuario,
                Nombre = nombre,
                IncluirInactivos = incluirInactivos,
                Usuarios = usuarios
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearEditarUsuario(string usuario = null)
        {
            CrearEditarUsuarioDto model = new CrearEditarUsuarioDto();

            if (string.IsNullOrEmpty(usuario) == false)
            {
                Usuario usuarioExistente = await _usuariosService.GetUsuarioAsync(usuario);

                model = _mapper.Map<CrearEditarUsuarioDto>(usuarioExistente);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GuardarUsuario(CrearEditarUsuarioDto crearEditarUsuarioDto)
        {
            List<string> errores = new List<string>();

            if (ModelState.IsValid == false)
            {
                errores = ModelState.Values.SelectMany(y => y.Errors.Select(h => h.ErrorMessage)).ToList();

                return Json(new {exito = false, errores});
            }

            try
            {
                await _usuariosService.CrearEditarUsuarioAsync(crearEditarUsuarioDto);

                //Si es nuevo y cambio la password
                if (crearEditarUsuarioDto.EsNuevo == false && crearEditarUsuarioDto.PassWordTexto != "dummypass")
                {
                    await _usuariosService.RestaurarPasswordAsync(crearEditarUsuarioDto.NombreUsuario,
                        crearEditarUsuarioDto.PassWordTexto);
                }
            }
            catch (SistemaEncomiendaException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                errores.Add(e.Message);
                return Json(new { exito = false, errores = errores });
            }

            return Json(new
            {
                exito = true,
                newUrl = Url.Action("CrearEditarUsuario", new {usuario = crearEditarUsuarioDto.NombreUsuario})
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleEstadoUsuario(string usuario)
        {
            await _usuariosService.DesactivarActivarUsuarioAsync(usuario);

            return Json(new {exito = true});
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Paquetes(ListaPaquetesVm listaPaquetesVm)
        {
            DateTime fechaIniFiltro = new DateTime();
            DateTime fechaFinFiltro = new DateTime();

            if (string.IsNullOrEmpty(listaPaquetesVm.FechaInicioEntrega))
            {
                fechaIniFiltro = DateTime.Today;
                listaPaquetesVm.FechaInicioEntrega = DateTime.Today.ToString("d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fechaIniFiltro = DateTime.ParseExact(
                    listaPaquetesVm.FechaInicioEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);

            }

            if (string.IsNullOrEmpty(listaPaquetesVm.FechaFinEntrega))
            {
                fechaFinFiltro = DateTime.Today.AddDays(1).AddSeconds(-1);
                listaPaquetesVm.FechaFinEntrega = DateTime.Today.ToString("d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fechaFinFiltro = DateTime.ParseExact(
                    listaPaquetesVm.FechaFinEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);

                fechaFinFiltro = fechaFinFiltro.AddDays(1).AddSeconds(-1);
            }

            listaPaquetesVm.Paquetes = await _paqueteService.GetListaPaquetes(fechaIniFiltro, fechaFinFiltro,
                listaPaquetesVm.EstadoPaquete, listaPaquetesVm.LugarEntrega);


            return View(listaPaquetesVm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearEditarPaquete(int codigoPaquete = 0)
        {
           CrearEditarPaqueteVm model = new CrearEditarPaqueteVm();

            model.CrearEditarPaqueteDto = new CrearEditarPaqueteDto();

            //Si el codigo de paquete no es cero, se intenta mapear
            if (codigoPaquete != 0)
            {
                Paquete paquete = await _paqueteService.GetPaquete(codigoPaquete);
                model.CrearEditarPaqueteDto = _mapper.Map<CrearEditarPaqueteDto>(paquete);
                model.CrearEditarPaqueteDto.FechaEntregaTexto = paquete.FechaEntrega.ToString("d/M/yyyy");
            }
            else
            {
                model.CrearEditarPaqueteDto.FechaEntregaTexto = DateTime.Today.ToString("d/M/yyyy");
            }

            model.Vendedores = await _usuariosService.GetListaUsuariosAsync("", TipoUsuario.Vendedor);
            model.Clientes = await _usuariosService.GetListaUsuariosAsync("", TipoUsuario.Cliente);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GuardarPaquete(CrearEditarPaqueteDto crearEditarPaqueteDto)
        {
            List<string> errores = new List<string>();

            if (ModelState.IsValid == false)
            {
                errores = ModelState.Values.SelectMany(y => y.Errors.Select(h => h.ErrorMessage)).ToList();

                return Json(new { exito = false, errores });
            }

            crearEditarPaqueteDto.FechaEntrega = DateTime.ParseExact(
                crearEditarPaqueteDto.FechaEntregaTexto,
                "d/M/yyyy",
                CultureInfo.InvariantCulture);

            try
            {
                Paquete paquete = await _paqueteService.CrearEditarPaqueteAsync(crearEditarPaqueteDto);

                return Json(new
                    {exito = true, newUrl = Url.Action("CrearEditarPaquete", new {codigoPaquete = paquete.Codigo})});
            }
            catch (SistemaEncomiendaException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                errores.Add(e.Message);
                return Json(new { exito = false, errores = errores });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarcarPaqueteAnulado(int idPaquete)
        {
            await _paqueteService.MarcarPaqueteAnulado(idPaquete, DateTime.Now);

            return Json(new {exito = true});
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarcarPaqueteEntregado(int idPaquete, string fecha)
        {
            DateTime fechaEntregado = string.IsNullOrEmpty(fecha)
                ? DateTime.Now
                : DateTime.ParseExact(
                    fecha,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);


            await _paqueteService.MarcarPaqueteEntregado(idPaquete, fechaEntregado);

            return Json(new { exito = true });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerarReporte(ListaPaquetesVm listaPaquetesVm)
        {
            DateTime fechaIniFiltro;
            DateTime fechaFinFiltro;

            //Debemos hacer una transformacion para evitar problemas de problemas
            listaPaquetesVm.FechaInicioEntrega = listaPaquetesVm.FechaInicioEntrega.Replace("p. m.", "PM");
            listaPaquetesVm.FechaInicioEntrega = listaPaquetesVm.FechaInicioEntrega.Replace("a. m.", "AM");

            listaPaquetesVm.FechaFinEntrega = listaPaquetesVm.FechaFinEntrega.Replace("a. m.", "AM");
            listaPaquetesVm.FechaFinEntrega = listaPaquetesVm.FechaFinEntrega.Replace("p. m.", "PM");

            if (string.IsNullOrEmpty(listaPaquetesVm.FechaInicioEntrega))
            {
                fechaIniFiltro = DateTime.Today;
                listaPaquetesVm.FechaInicioEntrega = DateTime.Today.ToString("d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fechaIniFiltro = DateTime.ParseExact(
                    listaPaquetesVm.FechaInicioEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);

            }

            if (string.IsNullOrEmpty(listaPaquetesVm.FechaFinEntrega))
            {
                fechaFinFiltro = DateTime.Today.AddDays(1).AddSeconds(-1);
                listaPaquetesVm.FechaFinEntrega = DateTime.Today.ToString("d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fechaFinFiltro = DateTime.ParseExact(
                    listaPaquetesVm.FechaFinEntrega,
                    "d/M/yyyy",
                    CultureInfo.InvariantCulture);

                fechaFinFiltro = fechaFinFiltro.AddDays(1).AddSeconds(-1);
            }

            listaPaquetesVm.Paquetes = await _paqueteService.GetListaPaquetes(fechaIniFiltro, fechaFinFiltro,
                listaPaquetesVm.EstadoPaquete, listaPaquetesVm.LugarEntrega);

            //return View("_ReportePaquetes", listaPaquetesVm);
            

            string viewString = await _viewRenderService.RenderToStringAsync("Administracion/_ReportePaquetes", listaPaquetesVm);

            _logger.LogWarning("Ya estamos por mandar a generar el PDF");
            HtmlToPdf converter = new HtmlToPdf();

            Stream stream = new MemoryStream();
            PdfDocument doc = converter.ConvertHtmlString(viewString);

            doc.Save(stream);
            stream.Position = 0;
            doc.Close();
            _logger.LogWarning("Al parecer hemos llegado al punto donde ya se puede regresar el archivo");

            return File(stream, "application/octet-stream", "Reporte.pdf");

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Estadisticas()
        {
            EstadisticasVm model = new EstadisticasVm()
            {
                PaquetesEntregados = await _paqueteService.GetCantidadPaquetesEntregados(),
                PaquetesAnulados = await _paqueteService.GetCantidadPaquetesAnulados(),
                PaquetesPendientes = await _paqueteService.GetCantidadPaquetesPendientes(),
                CantidadClientes = await _usuariosService.GetCantidadClientes()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetGraficaPaquetesPasados(int cantidadMesesAtras)
        {
            List<ElementoSimpleGraficaVm> elemetosGrafica = new List<ElementoSimpleGraficaVm>();

            //Lo primero es crear un rango de tiempo desde este mes hacia N meses atras
            for (int i = cantidadMesesAtras -1; i >= 0; i--)
            {
                //En i tenemos el mes actual
                DateTime fechaObjetivo = DateTime.Now.AddMonths(-i);

                //Extraigamos su mes
                string mes = fechaObjetivo.ToString("MMM", CultureInfo.CurrentCulture); //Esto nos da su nombre corto: Abr, May, Jul, etc

                mes = mes.Replace(".", string.Empty);
                mes = mes.ToUpper();

                //Ahora hay que ir a traer cuantos paquetes entraron en ese mes
                int cantidad = await _paqueteService.GetCantidadPaquetesMes(fechaObjetivo);

                //Construimos el objeto 
                ElementoSimpleGraficaVm elemento = new ElementoSimpleGraficaVm()
                {
                    Texto = mes,
                    Valor = cantidad
                };

                elemetosGrafica.Add(elemento);

            }

            //Regresamos el json
            return Json(new {data = elemetosGrafica});
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CargaExcel()
        {
            //Regresa la view 
            AsignarExcelUsuarioVm excelUsuarioVm = new AsignarExcelUsuarioVm();
            excelUsuarioVm.Usuarios = await _usuariosService.GetListaUsuariosAsync();

            ViewBag.errores = TempData["errores"];
            ViewBag.exito = TempData["exito"];
            return View(excelUsuarioVm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GuardarExcelUsuario(AsignarExcelUsuarioVm model)
        {
            //Primero validar
            List<string> errores = new List<string>();

            if (ModelState.IsValid == false)
            {
                errores = ModelState.Values.SelectMany(y => y.Errors.Select(h => h.ErrorMessage)).ToList();

                TempData["errores"] = errores;

                return RedirectToAction(nameof(CargaExcel));
            }

            //Aca ha pasado las validaciones base. Procedemos a validar que realmente sube archivos validos
            if (model.Archivo.ContentType != "application/vnd.ms-excel" && model.Archivo.ContentType !=
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                errores.Add("Debes subir un archivo de excel valido");
                TempData["errores"] = errores;

                return RedirectToAction(nameof(CargaExcel));
            }

            if(model.Imagenes != null && model.Imagenes.Any(x => x.ContentType != "image/jpeg" && x.ContentType != "image/bmp" && x.ContentType != "image/png"))
            {
                errores.Add("Debes subir un archivo de imagen valido");
                TempData["errores"] = errores;

                return RedirectToAction(nameof(CargaExcel));
            }

            //Al llegar aca decimos que la data viene correctamente, asi que procedemos a insertarla

            //Primero limpiamos de Azure
            StorageCredentials storageCredentials = new StorageCredentials(
                _configuration["AzureStorageConfig:AccountName"], _configuration["AzureStorageConfig:AccountKey"]);

            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container =
                blobClient.GetContainerReference(_configuration["AzureStorageConfig:Container"]);

            //Saco una lista de multimedias usuario actuales
            List<MultimediaUsuario> multimediaUsuarios = await _usuariosService.MultimediasUsuario(model.IdUsuario);

            //Las recorro para eliminarlas del Azure
            foreach (MultimediaUsuario multimediaUsuario in multimediaUsuarios)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(multimediaUsuario.NombreArchivo);
                await blockBlob.DeleteIfExistsAsync();
            }

            //Luego de eliminarlas elimino las multimedias del usuario
            await _usuariosService.LimpiarMultimedias(model.IdUsuario);

            //Ahora procedo a subir las nuevas multimedias

            string nombreExcel = model.IdUsuario  + "-" + DateTime.Now.ToShortDateString() + "-" + Path.GetFileName(model.Archivo.FileName);
            nombreExcel = nombreExcel.Replace('/', '-');

            //Primero subo el excel
            CloudBlockBlob blobExcel = container.GetBlockBlobReference(nombreExcel);

            using (Stream stream = model.Archivo.OpenReadStream())
            {
                await blobExcel.UploadFromStreamAsync(stream);
            }

            //En este punto el excel se subio correctamente, procedemos a crear el archivo multimedia
            MultimediaUsuario multimediaUsuarioExcel = new MultimediaUsuario()
            {
                IdUsuario = model.IdUsuario,
                MimeType = model.Archivo.ContentType,
                NombreArchivo = nombreExcel,
                TipoArchivo = TipoArchivo.Excel
            };

            //Mandamos a guardar el archivo a la base
            await _usuariosService.GuardarMultimedias(new List<MultimediaUsuario>(){ multimediaUsuarioExcel });

            if (model.Imagenes != null)
            {
                //Procedemos a procesar las imagenes
                int contador = 0;

                foreach (IFormFile imagen in model.Imagenes)
                {
                    contador++;

                    string nombreImagen = model.IdUsuario + "(" + contador + ")" + "-" +
                                          DateTime.Now.ToShortDateString() + "-" + Path.GetFileName(imagen.FileName);

                    nombreImagen = nombreImagen.Replace("/", "-");

                    CloudBlockBlob blobImg = container.GetBlockBlobReference(nombreImagen);

                    using (Stream stream = imagen.OpenReadStream())
                    {
                        await blobImg.UploadFromStreamAsync(stream);
                    }

                    MultimediaUsuario multimediaUsuarioImg = new MultimediaUsuario()
                    {
                        IdUsuario = model.IdUsuario,
                        MimeType = imagen.ContentType,
                        NombreArchivo = nombreImagen,
                        TipoArchivo = TipoArchivo.Imagen
                    };

                    await _usuariosService.GuardarMultimedias(new List<MultimediaUsuario>() { multimediaUsuarioImg });
                }
            }

            //En este punto, se subieron all los ficheros, lo retorno informandole que fue un exito
            TempData["exito"] = true;

            return RedirectToAction(nameof(CargaExcel));


        }
    }
}