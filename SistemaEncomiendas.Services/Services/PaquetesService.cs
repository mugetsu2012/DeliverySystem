using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using SistemaEncomiendas.Core.Data.Repositories;
using SistemaEncomiendas.Core.DTO;
using SistemaEncomiendas.Core.Exceptions;
using SistemaEncomiendas.Core.Models;
using SistemaEncomiendas.Core.Services;

namespace SistemaEncomiendas.Services.Services
{
    public class PaquetesService: IPaqueteService
    {
        private readonly IPaqueteRepository _paqueteRepository;
        private readonly IMapper _mapper;

        public PaquetesService(IPaqueteRepository paqueteRepository, IMapper mapper)
        {
            _paqueteRepository = paqueteRepository;
            _mapper = mapper;
        }

        public async Task<Paquete> CrearEditarPaqueteAsync(CrearEditarPaqueteDto crearEditarPaqueteDto)
        {
            #region Validaciones

            //Lo primero que debemos hacer es validar

            //El precio no puede ser cero o menor
            if (crearEditarPaqueteDto.Precio <= 0)
            {
                throw new SistemaEncomiendaException("Debes ingresar un precio mayor a cero para el paquete");
            }

            //Si dejo el lugar de entrega vacio, kaboom!
            if (string.IsNullOrEmpty(crearEditarPaqueteDto.LugarEntrega))
            {
                throw new SistemaEncomiendaException("Debes indicar el lugar de entrega del paquete");
            }

            //Validamos que la fecha de entrega no puede ser menor a la fecha actual
            if (crearEditarPaqueteDto.FechaEntrega < DateTime.Today)
            {
                throw new SistemaEncomiendaException("La fecha de entrega debe ser mayor a la fecha actual");
            }

            #endregion

            //Ahora vemos si es un paquete nuevo o estamos editando uno ya creado

            if (crearEditarPaqueteDto.Codigo == 0)
            {
                //Entra aca cuando el paquete es nuevo
                Paquete nuevoPaquete = _mapper.Map<Paquete>(crearEditarPaqueteDto);

                await _paqueteRepository.Create(nuevoPaquete);

                return nuevoPaquete;
            }
            else
            {
                //Como ya es un paquete que existe, solo puedo modificar el paquete si es que esta pendiente
                Paquete paquete = await _paqueteRepository.GetByIdAsync(crearEditarPaqueteDto.Codigo);

                if (paquete.EstadoPaquete != EstadoPaquete.Pendiente)
                {
                    throw new SistemaEncomiendaException("No puedes modificar un paquete que ha sido anulado o entregado");
                }

                //Ahora, procedo a cambiar lo que me dijo
                _mapper.Map(crearEditarPaqueteDto, paquete);

                //Guardo a la base
                await _paqueteRepository.SaveChangesAsync();

                return paquete;
            }
        }

        public async Task MarcarPaqueteEntregado(int idPaquete, DateTime? fechaEntrega = null)
        {
            Paquete paquete = await _paqueteRepository.GetByIdAsync(idPaquete);

            paquete.FechaPaqueteEntregado = fechaEntrega ?? DateTime.Now;
            paquete.FechaCambioEstado = DateTime.Now;
            paquete.EstadoPaquete = EstadoPaquete.Entregado;

            await _paqueteRepository.SaveChangesAsync();
        }

        public async Task MarcarPaqueteAnulado(int idPaquete, DateTime? fechaAnulacion = null)
        {
            Paquete paquete = await _paqueteRepository.GetByIdAsync(idPaquete);

            paquete.FechaPaqueteAnulado = fechaAnulacion ?? DateTime.Now;
            paquete.FechaCambioEstado = DateTime.Now;
            paquete.EstadoPaquete = EstadoPaquete.Anulado;

            await _paqueteRepository.SaveChangesAsync();
        }

        public async Task<List<Paquete>> GetListaPaquetes(DateTime fechaInicio, DateTime fechaFin,
            EstadoPaquete? estadoPaquete = null, string lugarEntrega = "")
        {
            bool estadoNulo = estadoPaquete == null;
            bool lugarEntregaNull = string.IsNullOrEmpty(lugarEntrega);

            List<Paquete> paquetes = await _paqueteRepository.GetAll().Include(x => x.UsuarioEnvia)
                .Include(x => x.UsuarioRecibe).Where(x =>
                    (x.FechaEntrega >= fechaInicio && x.FechaEntrega <= fechaFin) &&
                    (estadoNulo || x.EstadoPaquete == estadoPaquete.Value) &&
                    (lugarEntregaNull || x.LugarEntrega.Contains(lugarEntrega))).ToListAsync();

            return paquetes;
        }

        public async Task<List<Paquete>> GetPaquetesCliente(DateTime fechaInicio, DateTime fechaFin, string usuario, string lugarEntrega = "", EstadoPaquete? estadoPaquete = null)
        {
            bool estadoNulo = estadoPaquete == null;
            bool lugarEntregaNull = string.IsNullOrEmpty(lugarEntrega);

            List<Paquete> paquetes = await _paqueteRepository.GetAll().Include(x => x.UsuarioRecibe)
                .Include(x => x.UsuarioEnvia).Where(x =>
                    (x.FechaEntrega >= fechaInicio && x.FechaEntrega <= fechaFin) && (x.IdUsuarioRecibe == usuario) &&
                    (x.UsuarioRecibe.TipoUsuario == TipoUsuario.Cliente) &&
                    (estadoNulo || x.EstadoPaquete == estadoPaquete.Value) &&
                    (lugarEntregaNull || x.LugarEntrega.Contains(lugarEntrega))).OrderBy(x => x.FechaEntrega)
                .ToListAsync();

            return paquetes;
        }

        public async Task<List<Paquete>> GetPaquetesVendedor(DateTime fechaInicio, DateTime fechaFin, string usuario, string lugarEntrega = "", EstadoPaquete? estadoPaquete = null, string cliente="")
        {
            bool estadoNulo = estadoPaquete == null;
            bool lugarEntregaNull = string.IsNullOrEmpty(lugarEntrega);
            bool clienteNull = string.IsNullOrEmpty(cliente);

            List<Paquete> paquetes = await _paqueteRepository.GetAll().Include(x => x.UsuarioRecibe)
                .Include(x => x.UsuarioEnvia).Where(x =>
                    (x.FechaEntrega >= fechaInicio && x.FechaEntrega <= fechaFin) && (x.IdUsuarioEnvia == usuario) &&
                    (x.UsuarioEnvia.TipoUsuario == TipoUsuario.Vendedor) &&
                    (estadoNulo || x.EstadoPaquete == estadoPaquete.Value) &&
                    (lugarEntregaNull || x.LugarEntrega.Contains(lugarEntrega))
                    && (clienteNull || x.IdUsuarioRecibe == cliente))
                .OrderBy(x => x.FechaEntrega)
                .ToListAsync();


            return paquetes;
        }

        public async Task<Paquete> GetPaquete(int idPaquete)
        {
            Paquete paquete = await _paqueteRepository.GetAll().Include(m => m.UsuarioRecibe)
                .Include(m => m.UsuarioEnvia).FirstOrDefaultAsync(x => x.Codigo == idPaquete);

            return paquete;
        }

        public async Task<int> GetCantidadPaquetesAnulados()
        {
            int cantidad = await _paqueteRepository.GetAll()
                .CountAsync(x => x.EstadoPaquete == EstadoPaquete.Anulado);

            return cantidad;
        }

        public async Task<int> GetCantidadPaquetesPendientes()
        {
            int cantidad = await _paqueteRepository.GetAll()
                .CountAsync(x => x.EstadoPaquete == EstadoPaquete.Pendiente);

            return cantidad;
        }

        public async Task<int> GetCantidadPaquetesEntregados()
        {
            int cantidad = await _paqueteRepository.GetAll()
                .CountAsync(x => x.EstadoPaquete == EstadoPaquete.Entregado);

            return cantidad;
        }

        public async Task<int> GetCantidadPaquetesMes(DateTime fecha)
        {
            //La fecha que es el inicio de nuestra cota
            DateTime fechaLimiteInicio = new DateTime(fecha.Year,fecha.Month,1);

            //En teoria esto deberia funcionar xD
            DateTime fechaLimiteFin = fechaLimiteInicio.AddMonths(1).AddDays(-1);

            int cantidad = await _paqueteRepository.GetAll()
                .CountAsync(x => x.FechaIngreso >= fechaLimiteInicio && x.FechaIngreso <= fechaLimiteFin);

            return cantidad;
        }
    }
}
