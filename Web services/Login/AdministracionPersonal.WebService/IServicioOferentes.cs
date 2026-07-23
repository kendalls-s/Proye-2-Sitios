using System.Collections.Generic;
using System.ServiceModel;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService
{
    /// <summary>
    /// Core2 - "Yo como administrador del sistema quiero un servicio que a
    /// partir de un codigo de puesto regrese los oferentes que cumplen los
    /// requisitos de este para obtener la lista de oferentes."
    ///
    /// Criterios de aceptacion:
    ///  - Recibe como parametro un codigo de puesto.
    ///  - Retorna un listado de oferentes indicando su nombre e identificacion.
    /// </summary>
    [ServiceContract]
    public interface IServicioOferentes
    {
        [OperationContract]
        List<OferenteApto> ObtenerOferentesAptos(string codigoPuesto);
    }
}
