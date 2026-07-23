using System.ServiceModel;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService
{
    /// <summary>
    /// Core4 - "Yo como administrador del sistema quiero un servicio que permita
    /// autenticar un usuario, para utilizar las opciones."
    ///
    /// Criterio de aceptacion: el servicio solicita un usuario y una contrasena
    /// y determina si estas credenciales son correctas o no.
    /// </summary>
    [ServiceContract]
    public interface IServicioAutenticacion
    {
        /// <summary>
        /// Valida las credenciales recibidas contra la tabla usuario.
        /// </summary>
        [OperationContract]
        ResultadoAutenticacion Autenticar(CredencialesUsuario credenciales);
    }
}
