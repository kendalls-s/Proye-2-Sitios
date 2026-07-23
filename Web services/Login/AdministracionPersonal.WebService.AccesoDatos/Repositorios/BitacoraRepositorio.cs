using System;
using System.Diagnostics;
using Dapper;

namespace AdministracionPersonal.WebService.AccesoDatos
{
    public class BitacoraRepositorio : RepositorioBase, IBitacoraRepositorio
    {
        public BitacoraRepositorio(string connectionString) : base(connectionString)
        {
        }

        public void Registrar(
            string tipo,
            string entidad,
            string descripcion,
            int? idUsuario,
            string datosAnteriores,
            string datosNuevos)
        {
            const string sql = @"
INSERT INTO bitacora (fecha, id_usuario, tipo, entidad, datos_anteriores, datos_nuevos, descripcion)
VALUES (@Fecha, @IdUsuario, @Tipo, @Entidad, @DatosAnteriores, @DatosNuevos, @Descripcion);";

            try
            {
                using (var conexion = CrearConexion())
                {
                    conexion.Execute(sql, new
                    {
                        Fecha = DateTime.Now,
                        IdUsuario = idUsuario,
                        Tipo = tipo,
                        Entidad = entidad,
                        DatosAnteriores = datosAnteriores,
                        DatosNuevos = datosNuevos,
                        Descripcion = descripcion
                    });
                }
            }
            catch (Exception ex)
            {
                // Un fallo al escribir la bitacora no debe interrumpir el flujo principal.
                Trace.TraceError("Error al registrar en bitacora ({0}): {1}", entidad, ex.Message);
            }
        }
    }
}
