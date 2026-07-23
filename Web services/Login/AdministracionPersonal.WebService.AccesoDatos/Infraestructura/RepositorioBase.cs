using System;
using System.Configuration;
using MySqlConnector;

namespace AdministracionPersonal.WebService.AccesoDatos
{
    public abstract class RepositorioBase
    {
        protected RepositorioBase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("La string de conexion no puede estar vacia.", nameof(connectionString));
            }

            ConnectionString = connectionString;
        }

        protected string ConnectionString { get; }

        protected static string ObtenerCadenaConexionDesdeConfig(string connectionStringName)
        {
            var setting = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    string.Format("No se encontro la string de conexion '{0}'.", connectionStringName));
            }

            return setting.ConnectionString;
        }

        protected MySqlConnection CrearConexion()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
