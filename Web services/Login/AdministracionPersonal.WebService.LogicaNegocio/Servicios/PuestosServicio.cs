using System;
using System.Collections.Generic;
using System.Linq;
using AdministracionPersonal.WebService.AccesoDatos;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.LogicaNegocio
{
    /// <summary>
    /// Core1 - "Yo como administrador del sistema quiero un servicio de listado
    /// de puestos para obtener los puestos activos."
    ///
    /// Sigue el mismo patrón que OferentesAptosServicio (Core2): retorna
    /// directamente la lista (sin envoltorio Resultado), registra la consulta
    /// en bitácora, y ante un error técnico lo deja en bitácora y relanza la
    /// excepción (no hay un estado de negocio que expresar aquí distinto de
    /// "hay puestos" / "no hay puestos").
    /// </summary>
    public class PuestosServicio : IPuestosServicio
    {
        private readonly IPuestoRepositorio _puestoRepositorio;
        private readonly IBitacoraRepositorio _bitacoraRepositorio;

        public PuestosServicio(string connectionString)
            : this(new PuestoRepositorio(connectionString), new BitacoraRepositorio(connectionString))
        {
        }

        public PuestosServicio(IPuestoRepositorio puestoRepositorio, IBitacoraRepositorio bitacoraRepositorio)
        {
            if (puestoRepositorio == null) throw new ArgumentNullException(nameof(puestoRepositorio));
            if (bitacoraRepositorio == null) throw new ArgumentNullException(nameof(bitacoraRepositorio));

            _puestoRepositorio = puestoRepositorio;
            _bitacoraRepositorio = bitacoraRepositorio;
        }

        public IEnumerable<PuestoActivo> ObtenerPuestosActivos()
        {
            try
            {
                var puestos = _puestoRepositorio.ObtenerActivos().ToList();

                Bitacora("SELECT", "El usuario consulta lista de puestos activos.");

                return puestos;
            }
            catch (Exception ex)
            {
                Bitacora("ERROR", string.Format("Error técnico al consultar puestos activos: {0}", ex.Message));
                throw;
            }
        }

        private void Bitacora(string tipo, string descripcion)
        {
            _bitacoraRepositorio.Registrar(tipo, "puesto", descripcion, null, null, null);
        }
    }
}
