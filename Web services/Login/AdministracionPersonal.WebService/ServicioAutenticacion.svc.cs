using System;
using System.Configuration;
using AdministracionPersonal.WebService.LogicaNegocio;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService
{
    /// <summary>
    /// Core4 - Implementacion del servicio de autenticacion.
    /// La clase solo expone la operacion: toda la regla de negocio vive en
    /// AdministracionPersonal.WebService.LogicaNegocio.
    /// </summary>
    public class ServicioAutenticacion : IServicioAutenticacion
    {
        private readonly IAutenticacionServicio _autenticacionServicio;

        public ServicioAutenticacion()
            : this(CrearServicio())
        {
        }

        internal ServicioAutenticacion(IAutenticacionServicio autenticacionServicio)
        {
            _autenticacionServicio = autenticacionServicio;
        }

        public ResultadoAutenticacion Autenticar(CredencialesUsuario credenciales)
        {
            return _autenticacionServicio.Autenticar(credenciales);
        }

        private static IAutenticacionServicio CrearServicio()
        {
            var conexion = ConfigurationManager.ConnectionStrings["AdministracionPersonalDb"];
            if (conexion == null || string.IsNullOrWhiteSpace(conexion.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "No se encontro la string de conexion 'AdministracionPersonalDb'.");
            }

            var claveAes = ConfigurationManager.AppSettings["Security:AesKey"];
            if (string.IsNullOrWhiteSpace(claveAes))
            {
                throw new ConfigurationErrorsException(
                    "No se encontro el appSetting 'Security:AesKey'. Debe ser la misma clave de 32 " +
                    "caracteres usada en el Alcance 1 para cifrar las contrasenas.");
            }

            int minutos;
            if (!int.TryParse(ConfigurationManager.AppSettings["Sesion:DuracionMinutos"], out minutos))
            {
                minutos = 60;
            }

            return new AutenticacionServicio(conexion.ConnectionString, claveAes, minutos);
        }
    }
}
