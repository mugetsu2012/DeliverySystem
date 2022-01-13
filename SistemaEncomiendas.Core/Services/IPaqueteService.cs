using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SistemaEncomiendas.Core.DTO;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Core.Services
{
    public interface IPaqueteService
    {
        /// <summary>
        /// Me permite crear o editar un paquete, este metodo no sirve para mover de estado al paquete
        /// </summary>
        /// <param name="crearEditarPaqueteDto"></param>
        /// <returns></returns>
        Task<Paquete> CrearEditarPaqueteAsync(CrearEditarPaqueteDto crearEditarPaqueteDto);

        /// <summary>
        /// Accion para marcar que el paquete se entrego
        /// </summary>
        /// <param name="idPaquete"></param>
        /// <param name="fechaEntrega"></param>
        Task MarcarPaqueteEntregado(int idPaquete, DateTime? fechaEntrega = null);

        /// <summary>
        /// Accion para marcar el paquete como anulado
        /// </summary>
        /// <param name="idPaquete"></param>
        /// <param name="fechaAnulacion"></param>
        Task MarcarPaqueteAnulado(int idPaquete, DateTime? fechaAnulacion = null);

        /// <summary>
        /// Me extrae una lista de paquetes por medio de un rango de fechas y el estado del paquete
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="estadoPaquete">Si es null extraer todos los paquetes</param>
        /// <param name="lugarEntrega"></param>
        /// <returns></returns>
        Task<List<Paquete>> GetListaPaquetes(DateTime fechaInicio, DateTime fechaFin, EstadoPaquete? estadoPaquete = null, string lugarEntrega = "");

        Task<List<Paquete>> GetPaquetesCliente(DateTime fechaInicio, DateTime fechaFin, string usuario, string lugarEntrega ="", EstadoPaquete? estadoPaquete = null);

        Task<List<Paquete>> GetPaquetesVendedor(DateTime fechaInicio, DateTime fechaFin, string usuario,
            string lugarEntrega = "", EstadoPaquete? estadoPaquete = null, string cliente = "");

        /// <summary>
        /// Regresa un paquete, incluye el usario que envia y el que recibe
        /// </summary>
        /// <param name="idPaquete"></param>
        /// <returns></returns>
        Task<Paquete> GetPaquete(int idPaquete);

        Task<int> GetCantidadPaquetesAnulados();

        Task<int> GetCantidadPaquetesPendientes();

        Task<int> GetCantidadPaquetesEntregados();

        Task<int> GetCantidadPaquetesMes(DateTime fecha);
    }
}
