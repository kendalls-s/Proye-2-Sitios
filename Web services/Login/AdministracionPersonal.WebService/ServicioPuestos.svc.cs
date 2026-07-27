using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AdministracionPersonal.WebService.LogicaNegocio;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService
{
    /// <summary>
    /// Core1 - Implementación del servicio de puestos.
    /// La clase solo expone la operación: toda la regla de negocio vive en
    /// AdministracionPersonal.WebService.LogicaNegocio.
    /// </summary>
    public class ServicioPuestos : IServicioPuestos
    {
        private readonly IPuestosServicio _puestosServicio;

        public ServicioPuestos()
            : this(CrearServicio())
        {
        }

        internal ServicioPuestos(IPuestosServicio puestosServicio)
        {
            _puestosServicio = puestosServicio;
        }

        public List<PuestoActivo> ObtenerPuestosActivos()
        {
            return _puestosServicio.ObtenerPuestosActivos().ToList();
        }

        private static IPuestosServicio CrearServicio()
        {
            var conexion = ConfigurationManager.ConnectionStrings["AdministracionPersonalDb"];
            if (conexion == null || string.IsNullOrWhiteSpace(conexion.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "No se encontro la string de conexion 'AdministracionPersonalDb'.");
            }

            return new PuestosServicio(conexion.ConnectionString);
        }
    }
}
