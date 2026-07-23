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
    }
}
