using System.Collections.Generic;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.LogicaNegocio
{
    /// <summary>
    /// Core1 - Reglas de negocio del listado de puestos activos.
    /// </summary>
    public interface IPuestosServicio
    {
        IEnumerable<PuestoActivo> ObtenerPuestosActivos();
    }

    /// <summary>
    /// Core3 - Reglas de negocio para el registro de un empleado.
    /// </summary>
    public interface IEmpleadosServicio
    {
        ResultadoCrearEmpleado CrearEmpleado(SolicitudCrearEmpleado solicitud);
    }

    /// <summary>
    /// Core2 - Reglas de negocio para obtener los oferentes aptos de un puesto.
    /// </summary>
    public interface IOferentesAptosServicio
    {
        /// <summary>
        /// Retorna los oferentes que cumplen los requisitos del puesto indicado.
        /// Si el puesto no existe, retorna una lista vacia (se deja registrado
        /// en bitacora que el codigo no fue encontrado).
        /// </summary>
        IEnumerable<OferenteApto> ObtenerOferentesAptos(string codigoPuesto);

        /// <summary>
        /// Retorna toda la informacion registrada del oferente cuya
        /// identificacion coincide. Si no existe, retorna null (se deja
        /// registrado en bitacora que la identificacion no fue encontrada).
        /// </summary>
        OferenteDetalle ObtenerDetalleOferente(string identificacion);
    }

    /// <summary>
    /// Core4 - Reglas de negocio de la autenticacion de usuarios.
    /// </summary>
    public interface IAutenticacionServicio
    {
        ResultadoAutenticacion Autenticar(CredencialesUsuario credenciales);
    }

    /// <summary>
    /// Verificacion de contrasenas replicando el esquema AES-GCM del Alcance 1.
    /// Formato almacenado en usuario.password_hash: AESGCM:iv:tag:cipherdata
    /// (cada parte en base64; iv de 12 bytes, tag de 16 bytes, AES-256).
    /// </summary>
    public interface ICriptografiaServicio
    {
        /// <summary>
        /// Indica si passwordPlano corresponde al valor cifrado passwordHash
        /// almacenado. Ante cualquier error retorna false.
        /// </summary>
        bool Verificar(string passwordPlano, string passwordHash);
    }
}
