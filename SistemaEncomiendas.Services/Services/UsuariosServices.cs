using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaEncomiendas.Core.Data.Repositories;
using SistemaEncomiendas.Core.DTO;
using SistemaEncomiendas.Core.Exceptions;
using SistemaEncomiendas.Core.Models;
using SistemaEncomiendas.Core.Providers;
using SistemaEncomiendas.Core.Services;

namespace SistemaEncomiendas.Services.Services
{
    public class UsuariosServices: IUsuariosService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMultimediaUsuarioRepository _multimediaUsuarioRepository;
        private readonly IMapper _mapper;
        private readonly ICifradoProvider _cifradoProvider;

        public UsuariosServices(
            IUsuarioRepository usuarioRepository,
            IMultimediaUsuarioRepository multimediaUsuarioRepository,
            IMapper mapper, 
            ICifradoProvider cifradoProvider)
        {
            _usuarioRepository = usuarioRepository;
            _multimediaUsuarioRepository = multimediaUsuarioRepository;
            _mapper = mapper;
            _cifradoProvider = cifradoProvider;
        }

        public async Task<Usuario> CrearEditarUsuarioAsync(CrearEditarUsuarioDto crearEditarUsuarioDto)
        {
            #region Validaciones

            //Si el nombre de usuario esta vacio
            if (string.IsNullOrEmpty(crearEditarUsuarioDto.NombreUsuario))
            {
                throw new SistemaEncomiendaException("El nombre de usuario es requerido");
            }

            if (string.IsNullOrEmpty(crearEditarUsuarioDto.Nombre))
            {
                throw new SistemaEncomiendaException("El nombre es requerido");
            }

            if (string.IsNullOrEmpty(crearEditarUsuarioDto.Apellido))
            {
                throw new SistemaEncomiendaException("El apellido es requerido");
            }

            //Cuando es nuevo el password es requerido
            if (crearEditarUsuarioDto.EsNuevo && string.IsNullOrEmpty(crearEditarUsuarioDto.PassWordTexto))
            {
                throw new SistemaEncomiendaException("El password es requerido");
            }
            #endregion

            if (crearEditarUsuarioDto.EsNuevo)
            {
                #region Creacion del usuario

                //Entra en este bloque cuando es un nuevo usuario

                //Lo primero que hacemos es validar que no exista el ususario
                Usuario usuario = await _usuarioRepository.GetByIdAsync(crearEditarUsuarioDto.NombreUsuario);

                if (usuario != null)
                {
                    //Como el usuario no es nulo, es decir, existe, explotamos!
                    throw new SistemaEncomiendaException($"El usuario {crearEditarUsuarioDto.NombreUsuario} ya existe, debes ingresar otro nombre de usuario");
                }

                //Aca llega y cada validacion ha sido aprobada, procedamos a crear al usuario
                Usuario nuevoUsuario = _mapper.Map<Usuario>(crearEditarUsuarioDto);

                //Ahora hay que encriptar el password en texto
                nuevoUsuario.PassWord = _cifradoProvider.CifrarTexto(crearEditarUsuarioDto.PassWordTexto);

                //Ahora procedemos a guardar el ususario
                return await _usuarioRepository.Create(nuevoUsuario);

                #endregion
            }
            else
            {
                #region Edicion del usuario

                //Entra en este cuando se esta editando el usuario

                //Primero sacamos el usuario

                Usuario usuario = await _usuarioRepository.GetByIdAsync(crearEditarUsuarioDto.NombreUsuario);

                //Ahora seteamos los campos que pueden cambiarse
                usuario.Nombre = crearEditarUsuarioDto.Nombre;
                usuario.Apellido = crearEditarUsuarioDto.Apellido;
                
                //Hacemos save changes

                await _usuarioRepository.SaveChangesAsync(); 

                //Regresamos el usuario modificado
                return usuario;

                #endregion
            }
        }

        public async Task DesactivarActivarUsuarioAsync(string usurio)
        {
            Usuario usuario = await _usuarioRepository.GetByIdAsync(usurio);

            usuario.Activo = !usuario.Activo;

            await _usuarioRepository.SaveChangesAsync();
        }

        public async Task<string> RestaurarPasswordAsync(string usuario, string passWordNuevo = null)
        {
            //Primero sacamos al usuario
            Usuario usuarioCambiar = await _usuarioRepository.GetByIdAsync(usuario);

            //Si me envian un password, le dejo eso sino, yo le genero uno nuevo
            passWordNuevo = string.IsNullOrEmpty(passWordNuevo) ? RandomString(8) : passWordNuevo;

            usuarioCambiar.PassWord = _cifradoProvider.CifrarTexto(passWordNuevo);

            //Vamos a hacer savechanges
            await _usuarioRepository.SaveChangesAsync();

            return passWordNuevo;
        }

        public async Task<Usuario> GetUsuarioAsync(string idUsuario)
        {
            return await _usuarioRepository.GetByIdAsNoTracking(x => x.NombreUsuario == idUsuario);
        }

        public async Task<List<Usuario>> GetListaUsuariosAsync(string nombre = "", TipoUsuario? tipoUsuario = null, bool incluirInactivos = false)
        {
            bool nombreVacio = string.IsNullOrEmpty(nombre);
            bool tipoUsuarioNull = tipoUsuario == null;

            List<Usuario> usuarios = await _usuarioRepository.GetListaAsync(x =>
                (x.NombreUsuario.Contains(nombre) || x.Nombre.Contains(nombre) || x.Apellido.Contains(nombre) ||
                 nombreVacio) && (tipoUsuarioNull || x.TipoUsuario == tipoUsuario.Value) &&
                (x.Activo || incluirInactivos) && (x.NombreUsuario != "dortiz"));

            return usuarios;
        }

        public async Task<bool> ValidarLoginAsync(string usuario, string password)
        {
            Usuario usuarioProbar = await _usuarioRepository.GetByIdAsNoTracking(x => x.NombreUsuario == usuario);

            //No existe el usuario
            if (usuarioProbar == null)
            {
                return false;
            }

            //El usuario esta inactivo
            if (usuarioProbar.Activo == false)
            {
                return false;
            }

            //Validar si las password coinciden

            byte[] passwordCifrada = _cifradoProvider.CifrarTexto(password);

            return usuarioProbar.PassWord.SequenceEqual(passwordCifrada);
        }

        public async Task<List<Usuario>> GetClientesVendedor(string idVendedor)
        {
            List<Usuario> usuarios =
                await _usuarioRepository.GetListaAsync(x => x.PaquetesRecibe.Any(y => y.IdUsuarioEnvia == idVendedor));

            return usuarios;
        }

        public async Task<int> GetCantidadClientes()
        {
            int cantidad = await _usuarioRepository.GetAll()
                .CountAsync(x => x.TipoUsuario == TipoUsuario.Cliente && x.Activo);

            return cantidad;
        }

        public async Task GuardarMultimedias(List<MultimediaUsuario> multimedias)
        {
            await _multimediaUsuarioRepository.Create(multimedias);
        }

        public async Task LimpiarMultimedias(string idUsuario)
        {
            List<MultimediaUsuario> multimediaUsuarios = await MultimediasUsuario(idUsuario);

            List<int> codigosEliminar = multimediaUsuarios.Select(x => x.Codigo).ToList();

            foreach (int codigoEliminar in codigosEliminar)
            {
                await _multimediaUsuarioRepository.Delete(codigoEliminar);
            }
        }

        public async Task<List<MultimediaUsuario>> MultimediasUsuario(string idUsuario)
        {
            return await _multimediaUsuarioRepository.GetListaAsync(x => x.IdUsuario == idUsuario);
        }

        private string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
