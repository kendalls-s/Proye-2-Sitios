using System.Collections.Generic;
using System.Linq;
using Dapper;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.AccesoDatos
{
    public class OferenteRepositorio : RepositorioBase, IOferenteRepositorio
    {
        public OferenteRepositorio(string connectionString) : base(connectionString)
        {
        }

        public bool ExistePuesto(string codigoPuesto)
        {
            const string sql = "SELECT 1 FROM puesto WHERE codigo = @Codigo LIMIT 1;";

            using (var conexion = CrearConexion())
            {
                return conexion.QueryFirstOrDefault<int?>(sql, new { Codigo = codigoPuesto }) != null;
            }
        }

        public IEnumerable<OferenteApto> ObtenerAptosPorPuesto(string codigoPuesto)
        {
            // La vista ya filtra: postulacion en estado RECIBIDA/EN_REVISION/APTO
            // y total_requisitos = requisitos_cumplidos (o sin requisitos definidos).
            const string sql = @"
SELECT id_oferente     AS IdOferente,
       identificacion  AS Identificacion,
       nombre_completo AS NombreCompleto
FROM vw_oferentes_aptos_puesto
WHERE codigo_puesto = @Codigo
ORDER BY nombre_completo;";

            using (var conexion = CrearConexion())
            {
                return conexion.Query<OferenteApto>(sql, new { Codigo = codigoPuesto }).ToList();
            }
        }

        public OferenteDetalle ObtenerDetallePorIdentificacion(string identificacion)
        {
            const string sqlOferente = @"
SELECT o.id_oferente     AS IdOferente,
       o.identificacion  AS Identificacion,
       o.tipo_identificacion AS TipoIdentificacion,
       o.nombre_completo AS NombreCompleto,
       o.fecha_nacimiento AS FechaNacimiento,
       o.direccion       AS Direccion,
       o.fecha_registro  AS FechaRegistro,
       d.nombre          AS NombreDistrito,
       c.nombre          AS NombreCanton,
       p.nombre          AS NombreProvincia
FROM oferente o
LEFT JOIN distrito d ON d.id_distrito = o.id_distrito
LEFT JOIN canton c ON c.id_canton = d.id_canton
LEFT JOIN provincia p ON p.id_provincia = c.id_provincia
WHERE o.identificacion = @Identificacion;";

            const string sqlCorreos = "SELECT correo FROM oferente_correo WHERE id_oferente = @IdOferente;";

            const string sqlTelefonos = "SELECT telefono FROM oferente_telefono WHERE id_oferente = @IdOferente;";

            const string sqlPreparacion = @"
SELECT ie.nombre     AS Institucion,
       pa.titulo     AS Titulo,
       pa.fecha_inicio AS FechaInicio,
       pa.fecha_fin  AS FechaFin
FROM preparacion_academica pa
JOIN institucion_educativa ie ON ie.id_institucion = pa.id_institucion
WHERE pa.id_oferente = @IdOferente;";

            const string sqlExperiencia = @"
SELECT empresa AS Empresa, puesto AS Puesto, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin
FROM experiencia_laboral
WHERE id_oferente = @IdOferente;";

            const string sqlCurriculums = @"
SELECT nombre_archivo AS NombreArchivo,
       ruta_archivo   AS RutaArchivo,
       tipo_archivo   AS TipoArchivo,
       tamano_bytes   AS TamanoBytes,
       fecha_carga    AS FechaCarga
FROM curriculum_oferente
WHERE id_oferente = @IdOferente;";

            const string sqlPostulaciones = @"
SELECT pu.codigo AS CodigoPuesto,
       pu.nombre AS NombrePuesto,
       po.fecha_postulacion AS FechaPostulacion,
       po.estado AS Estado,
       po.observacion AS Observacion
FROM postulacion po
JOIN puesto pu ON pu.id_puesto = po.id_puesto
WHERE po.id_oferente = @IdOferente;";

            using (var conexion = CrearConexion())
            {
                var detalle = conexion.QueryFirstOrDefault<OferenteDetalle>(sqlOferente, new { Identificacion = identificacion });
                if (detalle == null)
                {
                    return null;
                }

                var parametros = new { IdOferente = detalle.IdOferente };

                detalle.Correos = conexion.Query<string>(sqlCorreos, parametros).ToList();
                detalle.Telefonos = conexion.Query<string>(sqlTelefonos, parametros).ToList();
                detalle.PreparacionAcademica = conexion.Query<PreparacionAcademicaOferente>(sqlPreparacion, parametros).ToList();
                detalle.ExperienciaLaboral = conexion.Query<ExperienciaLaboralOferente>(sqlExperiencia, parametros).ToList();
                detalle.Curriculums = conexion.Query<CurriculumOferente>(sqlCurriculums, parametros).ToList();
                detalle.Postulaciones = conexion.Query<PostulacionOferente>(sqlPostulaciones, parametros).ToList();

                return detalle;
            }
        }
    }
}
