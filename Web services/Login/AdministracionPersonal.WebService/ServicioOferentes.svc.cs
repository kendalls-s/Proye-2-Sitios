using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AdministracionPersonal.WebService.LogicaNegocio;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService
{
    /// <summary>
    /// Core2 - Implementacion del servicio de oferentes aptos por puesto.
    /// La clase solo expone la operacion: toda la regla de negocio vive en
    /// AdministracionPersonal.WebService.LogicaNegocio.
    /// </summary>
    public class ServicioOferentes : IServicioOferentes
    {
        private readonly IOferentesAptosServicio _oferentesAptosServicio;

        public ServicioOferentes()
            : this(CrearServicio())
        {
        }

        internal ServicioOferentes(IOferentesAptosServicio oferentesAptosServicio)
        {
            _oferentesAptosServicio = oferentesAptosServicio;
        }

        public List<OferenteApto> ObtenerOferentesAptos(string codigoPuesto)
        {
            return _oferentesAptosServicio.ObtenerOferentesAptos(codigoPuesto).ToList();
        }

        public OferenteDetalle ObtenerDetalleOferente(string identificacion)
        {
            return _oferentesAptosServicio.ObtenerDetalleOferente(identificacion);
        }

        private static IOferentesAptosServicio CrearServicio()
        {
            var conexion = ConfigurationManager.ConnectionStrings["AdministracionPersonalDb"];
            if (conexion == null || string.IsNullOrWhiteSpace(conexion.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "No se encontro la string de conexion 'AdministracionPersonalDb'.");
            }

            return new OferentesAptosServicio(conexion.ConnectionString);
        }
    }
}
