using System.Collections.Generic;
using System.Linq;
using Dapper;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.AccesoDatos
{
    public class PuestoRepositorio : RepositorioBase, IPuestoRepositorio
    {
        public PuestoRepositorio(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<PuestoActivo> ObtenerActivos()
        {
            const string sql = @"
SELECT id_puesto AS IdPuesto, codigo AS Codigo, nombre AS Nombre
FROM vw_puestos_disponibles
ORDER BY nombre;";

            using (var conexion = CrearConexion())
            {
                return conexion.Query<PuestoActivo>(sql).ToList();
            }
        }
    }
}
