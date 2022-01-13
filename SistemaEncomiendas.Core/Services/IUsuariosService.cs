using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SistemaEncomiendas.Core.DTO;
using SistemaEncomiendas.Core.Models;

namespace SistemaEncomiendas.Core.Services
{
    public interface IUsuariosService
    {
        /// <summary>
        /// Metodo que me permite crear un usuario, se encarga de cifrar la contraseña usando un provider <see cref="SistemaEncomiendas.Core.Providers.ICifradoProvider"/>
        /// </summary>
        /// <param name="crearEditarUsuarioDto"></param>
        /// <returns></returns>
        Task<Usuario> CrearEditarUsuarioAsync(CrearEditarUsuarioDto crearEditarUsuarioDto);

        /// <summary>
        /// Hace un toogle al estado del usuario
        /// </summary>
        /// <param name="usurio"></param>
        Task DesactivarActivarUsuarioAsync(string usurio);

        /// <summary>
        /// Este metodo extrae el usuario, me le pone una nueva password aleatoria y regresa dicho password
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="passWordNuevo">Nueva password, es opcional sino el sistema se inventa uno</param>
        /// <returns></returns>
        Task<string> RestaurarPasswordAsync(string usuario, string passWordNuevo = null);

        /// <summary>
        /// Extrae el usuario con sus paquetes
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        Task<Usuario> GetUsuarioAsync(string idUsuario);

        /// <summary>
        /// Metodo para extraer una lista de usuarios
        /// </summary>
        /// <param name="nombre">Busca en el nombre de usuario, nombre y apellido</param>
        /// <param name="tipoUsuario">Busca segun el tipo de usuario, si es null saca todos, menos el admin</param>
        /// <param name="incluirInactivos">Indica si quiero ver los inactivos</param>
        /// <returns></returns>
        Task<List<Usuario>> GetListaUsuariosAsync(string nombre = "", TipoUsuario? tipoUsuario = null, bool incluirInactivos = false);

        Task<bool> ValidarLoginAsync(string usuario, string password);

        /// <summary>
        /// Regresa una lista de usuarios que alguna vez han estado como Clientes en algun paquete
        /// para este  vendedor
        /// </summary>
        /// <param name="idVendedor"></param>
        /// <returns></returns>
        Task<List<Usuario>> GetClientesVendedor(string idVendedor);

        Task<int> GetCantidadClientes();

        /// <summary>
        /// Guarda una lista de multimedias de un usuario. Solo deberia ser utilizado despues de insertar en Azure
        /// </summary>
        /// <param name="multimedias"></param>
        /// <returns></returns>
        Task GuardarMultimedias(List<MultimediaUsuario> multimedias);

        /// <summary>
        /// Para un usuario especifico le quita todas sus multimedias. Solo deberia ser llamado despues de
        /// haber borrado las imagenes de Azure
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        Task LimpiarMultimedias(string idUsuario);

        /// <summary>
        /// Regresa una lista de multimedias del usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        Task<List<MultimediaUsuario>> MultimediasUsuario(string idUsuario);
    }
}
