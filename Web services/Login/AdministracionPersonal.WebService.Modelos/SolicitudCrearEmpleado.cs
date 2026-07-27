using System;
using System.Runtime.Serialization;

namespace AdministracionPersonal.WebService.Modelos
{
    /// <summary>
    /// Core3 - Datos para registrar un empleado en el sistema.
    /// Equivalente al CrearEmpleadoRequest que usaba la API REST.
    /// </summary>
    [DataContract]
    public class SolicitudCrearEmpleado
    {
        [DataMember(Order = 1)]
        public int IdOferente { get; set; }

        [DataMember(Order = 2)]
        public int IdPuesto { get; set; }

        [DataMember(Order = 3)]
        public DateTime FechaIngreso { get; set; }

        /// <summary>
        /// Id del empleado que aprueba la acción de personal de contratación.
        /// La tabla accion_personal exige un aprobador (FK a empleado). Si no
        /// se indica, se registra al propio empleado recién creado como
        /// aprobador de su contratación (no hay, por ahora, un mecanismo de
        /// sesión de usuario propagado hasta este servicio).
        /// </summary>
        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public int? IdAprobador { get; set; }
    }
}
