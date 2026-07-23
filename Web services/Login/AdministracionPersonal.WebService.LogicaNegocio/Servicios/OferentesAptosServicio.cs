using System;
using System.Collections.Generic;
using System.Linq;
using AdministracionPersonal.WebService.AccesoDatos;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.LogicaNegocio
{
    /// <summary>
    /// Core2 - "Yo como administrador del sistema quiero un servicio que a
    /// partir de un codigo de puesto regrese los oferentes que cumplen los
    /// requisitos de este para obtener la lista de oferentes".
    /// </summary>
    public class OferentesAptosServicio : IOferentesAptosServicio
    {
        private readonly IOferenteRepositorio _oferenteRepositorio;
        private readonly IBitacoraRepositorio _bitacoraRepositorio;

        public OferentesAptosServicio(string connectionString)
            : this(new OferenteRepositorio(connectionString), new BitacoraRepositorio(connectionString))
        {
        }

        public OferentesAptosServicio(
            IOferenteRepositorio oferenteRepositorio,
            IBitacoraRepositorio bitacoraRepositorio)
        {
            if (oferenteRepositorio == null) throw new ArgumentNullException(nameof(oferenteRepositorio));
            if (bitacoraRepositorio == null) throw new ArgumentNullException(nameof(bitacoraRepositorio));

            _oferenteRepositorio = oferenteRepositorio;
            _bitacoraRepositorio = bitacoraRepositorio;
        }

        public IEnumerable<OferenteApto> ObtenerOferentesAptos(string codigoPuesto)
        {
            if (string.IsNullOrWhiteSpace(codigoPuesto))
            {
                Bitacora("ERROR", "Consulta de oferentes aptos rechazada: no se indico codigo de puesto.");
                return Enumerable.Empty<OferenteApto>();
            }

            var codigo = codigoPuesto.Trim();

            try
            {
                if (!_oferenteRepositorio.ExistePuesto(codigo))
                {
                    Bitacora("ERROR", string.Format("Consulta de oferentes aptos: el puesto '{0}' no existe.", codigo));
                    return Enumerable.Empty<OferenteApto>();
                }

                var oferentes = _oferenteRepositorio.ObtenerAptosPorPuesto(codigo).ToList();

                // Manejo de bitacoras: caso "consulta" -> "El usuario consulta <elemento>".
                Bitacora(
                    "SELECT",
                    string.Format("El usuario consulta oferentes aptos para el puesto '{0}'.", codigo));

                return oferentes;
            }
            catch (Exception ex)
            {
                Bitacora("ERROR", string.Format(
                    "Error tecnico al consultar oferentes aptos para el puesto '{0}': {1}", codigo, ex.Message));
                throw;
            }
        }

        private void Bitacora(string tipo, string descripcion)
        {
            // idUsuario null: el criterio de aceptacion de Core2 no recibe un
            // token/usuario autenticado como parametro del servicio.
            _bitacoraRepositorio.Registrar(tipo, "oferente", descripcion, null, null, null);
        }
    }
}
