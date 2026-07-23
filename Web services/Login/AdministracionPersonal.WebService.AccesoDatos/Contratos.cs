using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.AccesoDatos
{
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
