using System.Runtime.Serialization;

namespace AdministracionPersonal.WebService.Modelos
{
    /// <summary>
    /// Core1 - Puesto activo (disponible = 1), tomado de vw_puestos_disponibles.
    /// </summary>
    [DataContract]
    public class PuestoActivo
    {
        [DataMember(Order = 1)]
        public int IdPuesto { get; set; }

        [DataMember(Order = 2)]
        public string Codigo { get; set; }

        [DataMember(Order = 3)]
        public string Nombre { get; set; }
    }
}
