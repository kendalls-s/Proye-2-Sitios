using System.Collections.Generic;
using System.ServiceModel;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService
{
    /// <summary>
    /// Core1 - "Yo como administrador del sistema quiero un servicio de listado
    /// de puestos para obtener los puestos activos."
    ///
    /// Criterio de aceptación: retorna un listado de puestos disponibles
    /// indicando su código y su nombre.
    /// </summary>
    [ServiceContract]
    public interface IServicioPuestos
    {
        [OperationContract]
        List<PuestoActivo> ObtenerPuestosActivos();
    }
}
