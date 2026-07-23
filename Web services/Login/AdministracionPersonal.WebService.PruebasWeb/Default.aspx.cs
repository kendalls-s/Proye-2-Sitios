using System;
using System.Configuration;
using System.Web.UI;
using AdministracionPersonal.WebService.PruebasWeb.Servicios;

namespace AdministracionPersonal.WebService.PruebasWeb
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtServicioUrl.Text = ConfigurationManager.AppSettings["ServicioAutenticacionUrl"] ?? string.Empty;
                MostrarMensaje("Ingresa la URL del servicio y unas credenciales para probar la operación Autenticar.", false);
            }
        }

        protected void btnAutenticar_Click(object sender, EventArgs e)
        {
            try
            {
                var resultado = ServicioAutenticacionCliente.Autenticar(
                    txtServicioUrl.Text.Trim(), txtUsuario.Text.Trim(), txtPassword.Text);

                litAutenticado.Text = resultado.Autenticado ? "Sí" : "No";
                litMensajeServicio.Text = Server.HtmlEncode(resultado.Mensaje ?? string.Empty);
                litIdUsuario.Text = resultado.IdUsuario.ToString();
                litNombreCompleto.Text = Server.HtmlEncode(resultado.NombreCompleto ?? string.Empty);
                litToken.Text = Server.HtmlEncode(resultado.Token ?? string.Empty);
                litExpira.Text = resultado.Expira == DateTime.MinValue
                    ? string.Empty
                    : resultado.Expira.ToString("yyyy-MM-dd HH:mm:ss");

                MostrarMensaje(
                    resultado.Autenticado ? "Credenciales correctas." : resultado.Mensaje,
                    !resultado.Autenticado);
            }
            catch (Exception ex)
            {
                LimpiarResultado();
                MostrarMensaje(ex.Message, true);
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtUsuario.Text = string.Empty;
            txtPassword.Text = string.Empty;
            LimpiarResultado();
            MostrarMensaje("Formulario limpio.", false);
        }

        private void LimpiarResultado()
        {
            litAutenticado.Text = string.Empty;
            litMensajeServicio.Text = string.Empty;
            litIdUsuario.Text = string.Empty;
            litNombreCompleto.Text = string.Empty;
            litToken.Text = string.Empty;
            litExpira.Text = string.Empty;
        }

        private void MostrarMensaje(string mensaje, bool error)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = error ? "mensaje error" : "mensaje ok";
        }
    }
}
