using System.Runtime.Serialization;

namespace AdministracionPersonal.WebService.Modelos
{
    /// <summary>
    /// Core2 - Un oferente que cumple los requisitos de un puesto.
    /// El criterio de aceptacion solo exige nombre e identificacion; se
    /// incluye IdOferente ademas porque Core7 (pantalla que consume este
    /// servicio) lo necesita para armar el enlace hacia el detalle (Core9).
    /// </summary>
    [DataContract]
    public class OferenteApto
    {
        [DataMember(Order = 1)]
        public int IdOferente { get; set; }

        [DataMember(Order = 2)]
        public string Identificacion { get; set; }

        [DataMember(Order = 3)]
        public string NombreCompleto { get; set; }
    }
}
