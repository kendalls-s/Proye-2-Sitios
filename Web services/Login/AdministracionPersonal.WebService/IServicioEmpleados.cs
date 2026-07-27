using System.ServiceModel;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService
{
    /// <summary>
    /// Core3 - "Yo como administrador del sistema quiero un servicio que
    /// permita registrar un empleado en el sistema."
    ///
    /// Criterio de aceptación: recibe en su cuerpo toda la información
    /// requerida para crear un nuevo empleado.
    /// </summary>
    [ServiceContract]
    public interface IServicioEmpleados
    {
        [OperationContract]
        ResultadoCrearEmpleado CrearEmpleado(SolicitudCrearEmpleado solicitud);
    }
}
