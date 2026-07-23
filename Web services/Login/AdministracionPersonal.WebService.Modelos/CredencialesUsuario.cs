using System.Runtime.Serialization;

namespace AdministracionPersonal.WebService.Modelos
{
    /// <summary>
    /// Core4 - Credenciales que recibe el servicio de autenticacion.
    /// Equivalente al LoginRequest que usaba la API REST.
    /// </summary>
    [DataContract]
    public class CredencialesUsuario
    {
        [DataMember(Order = 1)]
        public string Usuario { get; set; }

        [DataMember(Order = 2)]
        public string Password { get; set; }
    }
}
