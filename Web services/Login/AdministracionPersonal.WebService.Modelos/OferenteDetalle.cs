using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AdministracionPersonal.WebService.Modelos
{
    /// <summary>
    /// Detalle de oferente - "Yo como usuario del sistema quiero un servicio de
    /// detalle de oferente para obtener el detalle de la informacion de un oferente".
    ///
    /// Criterios de aceptacion:
    ///  - Retorna toda la informacion registrada para un oferente.
    ///  - Recibe como parametro un identificador de oferente. La tabla oferente no
    ///    tiene columna "codigo", por lo que se usa "identificacion" (unica) en su lugar.
    /// </summary>
    [DataContract]
    public class OferenteDetalle
    {
        [DataMember(Order = 1)]
        public int IdOferente { get; set; }

        [DataMember(Order = 2)]
        public string Identificacion { get; set; }

        [DataMember(Order = 3)]
        public string TipoIdentificacion { get; set; }

        [DataMember(Order = 4)]
        public string NombreCompleto { get; set; }

        [DataMember(Order = 5)]
        public DateTime FechaNacimiento { get; set; }

        [DataMember(Order = 6)]
        public string Direccion { get; set; }

        [DataMember(Order = 7)]
        public string NombreDistrito { get; set; }

        [DataMember(Order = 8)]
        public string NombreCanton { get; set; }

        [DataMember(Order = 9)]
        public string NombreProvincia { get; set; }

        [DataMember(Order = 10)]
        public DateTime FechaRegistro { get; set; }

        [DataMember(Order = 11)]
        public List<string> Correos { get; set; }

        [DataMember(Order = 12)]
        public List<string> Telefonos { get; set; }

        [DataMember(Order = 13)]
        public List<PreparacionAcademicaOferente> PreparacionAcademica { get; set; }

        [DataMember(Order = 14)]
        public List<ExperienciaLaboralOferente> ExperienciaLaboral { get; set; }

        [DataMember(Order = 15)]
        public List<CurriculumOferente> Curriculums { get; set; }

        [DataMember(Order = 16)]
        public List<PostulacionOferente> Postulaciones { get; set; }
    }

    [DataContract]
    public class PreparacionAcademicaOferente
    {
        [DataMember(Order = 1)]
        public string Institucion { get; set; }

        [DataMember(Order = 2)]
        public string Titulo { get; set; }

        [DataMember(Order = 3)]
        public DateTime FechaInicio { get; set; }

        [DataMember(Order = 4)]
        public DateTime FechaFin { get; set; }
    }

    [DataContract]
    public class ExperienciaLaboralOferente
    {
        [DataMember(Order = 1)]
        public string Empresa { get; set; }

        [DataMember(Order = 2)]
        public string Puesto { get; set; }

        [DataMember(Order = 3)]
        public DateTime FechaInicio { get; set; }

        [DataMember(Order = 4)]
        public DateTime FechaFin { get; set; }
    }

    [DataContract]
    public class CurriculumOferente
    {
        [DataMember(Order = 1)]
        public string NombreArchivo { get; set; }

        [DataMember(Order = 2)]
        public string RutaArchivo { get; set; }

        [DataMember(Order = 3)]
        public string TipoArchivo { get; set; }

        [DataMember(Order = 4)]
        public int? TamanoBytes { get; set; }

        [DataMember(Order = 5)]
        public DateTime FechaCarga { get; set; }
    }

    [DataContract]
    public class PostulacionOferente
    {
        [DataMember(Order = 1)]
        public string CodigoPuesto { get; set; }

        [DataMember(Order = 2)]
        public string NombrePuesto { get; set; }

        [DataMember(Order = 3)]
        public DateTime FechaPostulacion { get; set; }

        [DataMember(Order = 4)]
        public string Estado { get; set; }

        [DataMember(Order = 5)]
        public string Observacion { get; set; }
    }
}
