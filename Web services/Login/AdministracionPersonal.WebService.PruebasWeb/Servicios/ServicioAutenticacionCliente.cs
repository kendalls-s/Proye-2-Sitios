using System;
using System.ServiceModel;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.PruebasWeb.Servicios
{
    /// <summary>
    /// Cliente WCF creado por codigo (ChannelFactory), sin necesidad de agregar
    /// una referencia de servicio. Mismo enfoque del ejemplo del curso.
    /// </summary>
    internal static class ServicioAutenticacionCliente
    {
        public static ResultadoAutenticacion Autenticar(string urlServicio, string usuario, string password)
        {
            return Ejecutar(urlServicio, cliente => cliente.Autenticar(new CredencialesUsuario
            {
                Usuario = usuario,
                Password = password
            }));
        }

        private static T Ejecutar<T>(string urlServicio, Func<IServicioAutenticacion, T> accion)
        {
            if (string.IsNullOrWhiteSpace(urlServicio))
            {
                throw new ArgumentException("Ingresa la URL del servicio.", nameof(urlServicio));
            }

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                MaxReceivedMessageSize = 1024 * 1024,
                MaxBufferSize = 1024 * 1024
            };
            var endpoint = new EndpointAddress(urlServicio);
            var factory = new ChannelFactory<IServicioAutenticacion>(binding, endpoint);
            var channel = factory.CreateChannel();
            var communicationObject = (ICommunicationObject)channel;

            try
            {
                communicationObject.Open();
                var resultado = accion(channel);
                communicationObject.Close();
                factory.Close();
                return resultado;
            }
            catch
            {
                communicationObject.Abort();
                factory.Abort();
                throw;
            }
        }
    }
}
