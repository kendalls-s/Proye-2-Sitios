using System;
using System.Collections.Generic;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.AccesoDatos
{
    /// <summary>
    /// Core1 - Consultas sobre puestos activos.
    /// </summary>
    public interface IPuestoRepositorio
    {
        /// <summary>Puestos con disponible = 1, tomados de vw_puestos_disponibles.</summary>
        IEnumerable<PuestoActivo> ObtenerActivos();
    }

    /// <summary>
    /// Resultado interno (no expuesto por el servicio) de intentar crear un
    /// empleado. La capa de LogicaNegocio traduce cada código al mensaje en
    /// español exigido por los criterios de aceptación de Core3.
    /// </summary>
    public enum CodigoResultadoCrearEmpleado
    {
        Ok,
        OferenteNoExiste,
        PuestoNoExiste,
        PuestoNoDisponible,
        YaEsEmpleado
    }

    public class ResultadoInternoCrearEmpleado
    {
        public CodigoResultadoCrearEmpleado Codigo { get; set; }
        public int IdEmpleado { get; set; }
        public string NumeroEmpleado { get; set; }
    }

    /// <summary>
    /// Core3 - Alta de empleados a partir de un oferente existente.
    /// </summary>
    public interface IEmpleadoRepositorio
    {
        /// <summary>
        /// Ejecuta, dentro de una única transacción, todas las validaciones y el
        /// alta del empleado (tabla empleado + accion_personal de contratación).
        /// </summary>
        ResultadoInternoCrearEmpleado CrearEmpleado(
            int idOferente, int idPuesto, DateTime fechaIngreso, int? idAprobador);
    }

    /// <summary>
    /// Core2 - Consultas sobre oferentes en relacion a puestos.
    /// </summary>
    public interface IOferenteRepositorio
    {
        /// <summary>
        /// Verifica si existe un puesto con ese codigo (para distinguir
        /// "puesto inexistente" de "sin oferentes aptos" en la bitacora).
        /// </summary>
        bool ExistePuesto(string codigoPuesto);

        /// <summary>
        /// Retorna, a partir de vw_oferentes_aptos_puesto, los oferentes que
        /// se postularon al puesto y cumplen el 100% de sus requisitos.
        /// </summary>
        IEnumerable<OferenteApto> ObtenerAptosPorPuesto(string codigoPuesto);

        /// <summary>
        /// Retorna toda la informacion registrada del oferente cuya
        /// identificacion coincide exactamente, o null si no existe.
        /// </summary>
        OferenteDetalle ObtenerDetallePorIdentificacion(string identificacion);
    }

    public interface IUsuarioRepositorio
    {
        /// <summary>
        /// Busca el usuario SIN filtrar por estado, para poder distinguir
        /// "no existe" de "existe pero esta bloqueado/inactivo" en la bitacora.
        /// </summary>
        UsuarioAutenticacion ObtenerPorUsuario(string usuario);

        void ActualizarIntentos(int idUsuario, int intentos);

        void Bloquear(int idUsuario, int intentos);

        void RegistrarLoginExitoso(int idUsuario);
    }

    public interface IBitacoraRepositorio
    {
        /// <summary>
        /// Inserta un registro en la bitacora del sistema.
        /// </summary>
        /// <param name="tipo">INSERT | UPDATE | DELETE | SELECT | ERROR</param>
        /// <param name="entidad">Nombre de la tabla o entidad afectada.</param>
        /// <param name="descripcion">Descripcion legible de la accion.</param>
        /// <param name="idUsuario">Usuario que ejecuta la accion (null si no autenticado).</param>
        /// <param name="datosAnteriores">JSON con el estado previo (UPDATE/DELETE).</param>
        /// <param name="datosNuevos">JSON con el nuevo estado (INSERT/UPDATE).</param>
        void Registrar(
            string tipo,
            string entidad,
            string descripcion,
            int? idUsuario,
            string datosAnteriores,
            string datosNuevos);
    }
}
