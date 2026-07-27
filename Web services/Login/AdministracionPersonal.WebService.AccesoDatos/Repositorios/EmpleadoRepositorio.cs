using System;
using Dapper;

namespace AdministracionPersonal.WebService.AccesoDatos
{
    public class EmpleadoRepositorio : RepositorioBase, IEmpleadoRepositorio
    {
        public EmpleadoRepositorio(string connectionString) : base(connectionString)
        {
        }

        public ResultadoInternoCrearEmpleado CrearEmpleado(
            int idOferente, int idPuesto, DateTime fechaIngreso, int? idAprobador)
        {
            using (var conexion = CrearConexion())
            {
                conexion.Open();

                // No se llama transaccion.Rollback() explicitamente en ningun punto:
                // si el metodo retorna sin llegar a transaccion.Commit(), el "using"
                // de abajo revierte la transaccion automaticamente al hacer Dispose.
                using (var transaccion = conexion.BeginTransaction())
                {
                    var oferenteExiste = conexion.ExecuteScalar<int?>(
                        "SELECT id_oferente FROM oferente WHERE id_oferente = @IdOferente",
                        new { IdOferente = idOferente }, transaccion);

                    if (oferenteExiste == null)
                    {
                        return new ResultadoInternoCrearEmpleado { Codigo = CodigoResultadoCrearEmpleado.OferenteNoExiste };
                    }

                    var disponible = conexion.ExecuteScalar<int?>(
                        "SELECT disponible FROM puesto WHERE id_puesto = @IdPuesto FOR UPDATE",
                        new { IdPuesto = idPuesto }, transaccion);

                    if (disponible == null)
                    {
                        return new ResultadoInternoCrearEmpleado { Codigo = CodigoResultadoCrearEmpleado.PuestoNoExiste };
                    }

                    if (disponible == 0)
                    {
                        return new ResultadoInternoCrearEmpleado { Codigo = CodigoResultadoCrearEmpleado.PuestoNoDisponible };
                    }

                    var yaEsEmpleado = conexion.ExecuteScalar<int>(
                        "SELECT COUNT(1) FROM empleado WHERE id_oferente = @IdOferente",
                        new { IdOferente = idOferente }, transaccion);

                    if (yaEsEmpleado > 0)
                    {
                        return new ResultadoInternoCrearEmpleado { Codigo = CodigoResultadoCrearEmpleado.YaEsEmpleado };
                    }

                    var siguienteId = conexion.ExecuteScalar<int>(
                        "SELECT IFNULL(MAX(id_empleado), 0) + 1 FROM empleado FOR UPDATE",
                        transaction: transaccion);
                    var numeroEmpleado = string.Format("EMP-{0:D6}", siguienteId);

                    const string sqlInsertEmpleado = @"
INSERT INTO empleado (numero_empleado, id_oferente, id_puesto, fecha_ingreso)
VALUES (@NumeroEmpleado, @IdOferente, @IdPuesto, @FechaIngreso);";

                    conexion.Execute(sqlInsertEmpleado, new
                    {
                        NumeroEmpleado = numeroEmpleado,
                        IdOferente = idOferente,
                        IdPuesto = idPuesto,
                        FechaIngreso = fechaIngreso.Date
                    }, transaccion);

                    var idEmpleado = conexion.ExecuteScalar<int>(
                        "SELECT LAST_INSERT_ID()", transaction: transaccion);

                    var idAprobadorFinal = (idAprobador.HasValue && idAprobador.Value > 0)
                        ? idAprobador.Value
                        : idEmpleado;

                    const string sqlAccionPersonal = @"
INSERT INTO accion_personal (tipo_accion, fecha_accion, descripcion, id_empleado, id_aprobador)
VALUES ('CONTRATACION', @FechaAccion, @Descripcion, @IdEmpleado, @IdAprobador);";

                    conexion.Execute(sqlAccionPersonal, new
                    {
                        FechaAccion = fechaIngreso.Date,
                        Descripcion = string.Format(
                            "Contratación registrada mediante el sistema Core (número de empleado {0}).",
                            numeroEmpleado),
                        IdEmpleado = idEmpleado,
                        IdAprobador = idAprobadorFinal
                    }, transaccion);

                    transaccion.Commit();

                    return new ResultadoInternoCrearEmpleado
                    {
                        Codigo = CodigoResultadoCrearEmpleado.Ok,
                        IdEmpleado = idEmpleado,
                        NumeroEmpleado = numeroEmpleado
                    };
                }
            }
        }
    }
}
