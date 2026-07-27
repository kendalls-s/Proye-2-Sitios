using System.Configuration;
using AdministracionPersonal.WebService.LogicaNegocio;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService
{
    /// <summary>
    /// Core3 - Implementación del servicio de creación de empleados.
    /// La clase solo expone la operación: toda la regla de negocio vive en
    /// AdministracionPersonal.WebService.LogicaNegocio.
    /// </summary>
    public class ServicioEmpleados : IServicioEmpleados
    {
        private readonly IEmpleadosServicio _empleadosServicio;

        public ServicioEmpleados()
            : this(CrearServicio())
        {
        }

        internal ServicioEmpleados(IEmpleadosServicio empleadosServicio)
        {
            _empleadosServicio = empleadosServicio;
        }

        public ResultadoCrearEmpleado CrearEmpleado(SolicitudCrearEmpleado solicitud)
        {
            return _empleadosServicio.CrearEmpleado(solicitud);
        }

        private static IEmpleadosServicio CrearServicio()
        {
            var conexion = ConfigurationManager.ConnectionStrings["AdministracionPersonalDb"];
            if (conexion == null || string.IsNullOrWhiteSpace(conexion.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "No se encontro la string de conexion 'AdministracionPersonalDb'.");
            }

            bool mostrarDetalleError;
            bool.TryParse(ConfigurationManager.AppSettings["Debug:MostrarError"], out mostrarDetalleError);

            return new EmpleadosServicio(conexion.ConnectionString, mostrarDetalleError);
        }
    }
}
