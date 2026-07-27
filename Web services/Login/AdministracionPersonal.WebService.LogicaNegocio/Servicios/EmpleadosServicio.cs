using System;
using AdministracionPersonal.WebService.AccesoDatos;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.LogicaNegocio
{
    /// <summary>
    /// Core3 - "Yo como administrador del sistema quiero un servicio que permita
    /// registrar un empleado en el sistema."
    ///
    /// Sigue el mismo patrón que AutenticacionServicio (Core4): el resultado
    /// siempre viaja en un envoltorio Resultado{Verbo}/Mensaje, así que ante
    /// CUALQUIER error (de negocio o técnico) se captura y se devuelve un
    /// ResultadoCrearEmpleado.Fallido(...) en vez de dejar propagar un SOAP Fault.
    ///
    /// Reglas trasladadas tal cual desde EmpleadosController de la API REST:
    ///   - El oferente debe existir.
    ///   - El puesto debe existir y estar disponible.
    ///   - El oferente no puede tener ya un empleado asociado.
    ///   - El número de empleado se genera como "EMP-XXXXXX".
    ///   - Se crea la acción de personal de tipo CONTRATACION.
    ///   - Toda creación exitosa y todo error técnico quedan en la bitácora.
    /// </summary>
    public class EmpleadosServicio : IEmpleadosServicio
    {
        private readonly IEmpleadoRepositorio _empleadoRepositorio;
        private readonly IBitacoraRepositorio _bitacoraRepositorio;
        private readonly bool _mostrarDetalleError;

        public EmpleadosServicio(string connectionString, bool mostrarDetalleError = false)
            : this(new EmpleadoRepositorio(connectionString), new BitacoraRepositorio(connectionString), mostrarDetalleError)
        {
        }

        public EmpleadosServicio(
            IEmpleadoRepositorio empleadoRepositorio,
            IBitacoraRepositorio bitacoraRepositorio,
            bool mostrarDetalleError = false)
        {
            if (empleadoRepositorio == null) throw new ArgumentNullException(nameof(empleadoRepositorio));
            if (bitacoraRepositorio == null) throw new ArgumentNullException(nameof(bitacoraRepositorio));

            _empleadoRepositorio = empleadoRepositorio;
            _bitacoraRepositorio = bitacoraRepositorio;
            _mostrarDetalleError = mostrarDetalleError;
        }

        public ResultadoCrearEmpleado CrearEmpleado(SolicitudCrearEmpleado solicitud)
        {
            if (solicitud == null || solicitud.IdOferente <= 0 || solicitud.IdPuesto <= 0)
            {
                return ResultadoCrearEmpleado.Fallido("Debe indicar un oferente y un puesto válidos.");
            }

            var fechaIngreso = solicitud.FechaIngreso == default(DateTime)
                ? DateTime.Today
                : solicitud.FechaIngreso.Date;

            try
            {
                var resultado = _empleadoRepositorio.CrearEmpleado(
                    solicitud.IdOferente, solicitud.IdPuesto, fechaIngreso, solicitud.IdAprobador);

                switch (resultado.Codigo)
                {
                    case CodigoResultadoCrearEmpleado.OferenteNoExiste:
                        Bitacora("ERROR", string.Format(
                            "Creación de empleado rechazada: el oferente {0} no existe.", solicitud.IdOferente));
                        return ResultadoCrearEmpleado.Fallido("El oferente indicado no existe.");

                    case CodigoResultadoCrearEmpleado.PuestoNoExiste:
                        Bitacora("ERROR", string.Format(
                            "Creación de empleado rechazada: el puesto {0} no existe.", solicitud.IdPuesto));
                        return ResultadoCrearEmpleado.Fallido("El puesto indicado no existe.");

                    case CodigoResultadoCrearEmpleado.PuestoNoDisponible:
                        Bitacora("ERROR", string.Format(
                            "Creación de empleado rechazada: el puesto {0} no está disponible.", solicitud.IdPuesto));
                        return ResultadoCrearEmpleado.Fallido("El puesto indicado no está disponible.");

                    case CodigoResultadoCrearEmpleado.YaEsEmpleado:
                        Bitacora("ERROR", string.Format(
                            "Creación de empleado rechazada: el oferente {0} ya es empleado.", solicitud.IdOferente));
                        return ResultadoCrearEmpleado.Fallido("El oferente ya se encuentra registrado como empleado.");
                }

                Bitacora("INSERT", string.Format(
                    "Creación de nuevo empleado {0} (idEmpleado={1}, idOferente={2}, idPuesto={3}).",
                    resultado.NumeroEmpleado, resultado.IdEmpleado, solicitud.IdOferente, solicitud.IdPuesto));

                return new ResultadoCrearEmpleado
                {
                    Creado = true,
                    Mensaje = "Empleado creado con éxito",
                    IdEmpleado = resultado.IdEmpleado,
                    NumeroEmpleado = resultado.NumeroEmpleado,
                    IdOferente = solicitud.IdOferente,
                    IdPuesto = solicitud.IdPuesto,
                    FechaIngreso = fechaIngreso
                };
            }
            catch (Exception ex)
            {
                var detalle = ex.GetBaseException().Message;
                Bitacora("ERROR", string.Format(
                    "Error técnico al crear empleado a partir del oferente {0}: {1}", solicitud.IdOferente, detalle));

                var mensaje = _mostrarDetalleError
                    ? "Error técnico: " + detalle
                    : "Ocurrió un error al crear el empleado.";

                return ResultadoCrearEmpleado.Fallido(mensaje);
            }
        }

        private void Bitacora(string tipo, string descripcion)
        {
            _bitacoraRepositorio.Registrar(tipo, "empleado", descripcion, null, null, null);
        }
    }
}
