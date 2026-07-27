using System;
using System.Runtime.Serialization;

namespace AdministracionPersonal.WebService.Modelos
{
    /// <summary>
    /// Core3 - Respuesta del servicio de creación de empleados.
    /// Mismo patrón que ResultadoAutenticacion (Core4): cuando Creado es falso,
    /// Mensaje explica la razón exacta exigida por los criterios de aceptación.
    /// </summary>
    [DataContract]
    public class ResultadoCrearEmpleado
    {
        [DataMember(Order = 1)]
        public bool Creado { get; set; }

        [DataMember(Order = 2)]
        public string Mensaje { get; set; }

        [DataMember(Order = 3)]
        public int IdEmpleado { get; set; }

        [DataMember(Order = 4)]
        public string NumeroEmpleado { get; set; }

        [DataMember(Order = 5)]
        public int IdOferente { get; set; }

        [DataMember(Order = 6)]
        public int IdPuesto { get; set; }

        [DataMember(Order = 7)]
        public DateTime FechaIngreso { get; set; }

        public static ResultadoCrearEmpleado Fallido(string mensaje)
        {
            return new ResultadoCrearEmpleado
            {
                Creado = false,
                Mensaje = mensaje,
                IdEmpleado = 0,
                NumeroEmpleado = string.Empty,
                IdOferente = 0,
                IdPuesto = 0,
                FechaIngreso = DateTime.MinValue
            };
        }
    }
}
