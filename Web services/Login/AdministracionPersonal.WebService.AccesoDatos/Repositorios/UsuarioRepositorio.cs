using System;
using Dapper;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.AccesoDatos
{
    public class UsuarioRepositorio : RepositorioBase, IUsuarioRepositorio
    {
        public UsuarioRepositorio(string connectionString) : base(connectionString)
        {
        }

        public UsuarioAutenticacion ObtenerPorUsuario(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return null;
            }

            const string sql = @"
SELECT id_usuario      AS IdUsuario,
       usuario         AS Usuario,
       nombre_completo AS NombreCompleto,
       password_hash   AS PasswordHash,
       estado          AS Estado,
       intentos_login  AS IntentosLogin
FROM usuario
WHERE usuario = @Usuario;";

            using (var conexion = CrearConexion())
            {
                return conexion.QueryFirstOrDefault<UsuarioAutenticacion>(sql, new { Usuario = usuario });
            }
        }

        public void ActualizarIntentos(int idUsuario, int intentos)
        {
            const string sql = "UPDATE usuario SET intentos_login = @Intentos WHERE id_usuario = @Id;";

            using (var conexion = CrearConexion())
            {
                conexion.Execute(sql, new { Intentos = intentos, Id = idUsuario });
            }
        }

        public void Bloquear(int idUsuario, int intentos)
        {
            const string sql = @"
UPDATE usuario
SET intentos_login = @Intentos,
    estado         = 'BLOQUEADO',
    fecha_bloqueo  = @Ahora
WHERE id_usuario = @Id;";

            using (var conexion = CrearConexion())
            {
                conexion.Execute(sql, new { Intentos = intentos, Ahora = DateTime.Now, Id = idUsuario });
            }
        }

        public void RegistrarLoginExitoso(int idUsuario)
        {
            const string sql = @"
UPDATE usuario
SET intentos_login     = 0,
    fecha_ultimo_login = @Ahora
WHERE id_usuario = @Id;";

            using (var conexion = CrearConexion())
            {
                conexion.Execute(sql, new { Ahora = DateTime.Now, Id = idUsuario });
            }
        }
    }
}
