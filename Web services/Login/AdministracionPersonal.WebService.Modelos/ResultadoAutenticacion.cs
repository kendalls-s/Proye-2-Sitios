using System;
using System.Runtime.Serialization;

namespace AdministracionPersonal.WebService.Modelos
{
    /// <summary>
    /// Core4 - Respuesta del servicio de autenticacion.
    /// Cuando Autenticado es falso, Mensaje contiene el texto exigido por el
    /// criterio de aceptacion: "Usuario y/o contrasena incorrectos.".
    /// </summary>
    [DataContract]
    public class ResultadoAutenticacion
    {
        [DataMember(Order = 1)]
        public bool Autenticado { get; set; }

        [DataMember(Order = 2)]
        public string Mensaje { get; set; }

        [DataMember(Order = 3)]
        public int IdUsuario { get; set; }

        [DataMember(Order = 4)]
        public string NombreCompleto { get; set; }

        /// <summary>Token de sesion que el cliente guarda tras un login exitoso.</summary>
        [DataMember(Order = 5)]
        public string Token { get; set; }

        /// <summary>Fecha y hora (UTC) en la que expira el token.</summary>
        [DataMember(Order = 6)]
        public DateTime Expira { get; set; }

        public static ResultadoAutenticacion Fallido(string mensaje)
        {
            return new ResultadoAutenticacion
            {
                Autenticado = false,
                Mensaje = mensaje,
                IdUsuario = 0,
                NombreCompleto = string.Empty,
                Token = string.Empty,
                Expira = DateTime.MinValue
            };
        }
    }
}
