using System;
using System.Web.Script.Serialization;
using AdministracionPersonal.WebService.AccesoDatos;
using AdministracionPersonal.WebService.Modelos;

namespace AdministracionPersonal.WebService.LogicaNegocio
{
    /// <summary>
    /// Core4 - Autenticacion de usuarios.
    /// Reglas trasladadas tal cual desde AuthController de la API REST:
    ///   - Mismo mensaje para campos vacios y credenciales incorrectas.
    ///   - Usuario no ACTIVO: no se cuentan intentos ni se valida la contrasena.
    ///   - 3 intentos fallidos bloquean el usuario (HU Core5 / Seg1).
    ///   - Todas las acciones quedan registradas en la bitacora.
    /// </summary>
    public class AutenticacionServicio : IAutenticacionServicio
    {
        /// <summary>Mensaje unico exigido por el criterio de aceptacion.</summary>
        public const string MsgCredenciales = "Usuario y/o contraseña incorrectos.";

        private const int MaxIntentos = 3;
        private const string EstadoActivo = "ACTIVO";
        private const string EstadoBloqueado = "BLOQUEADO";

        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IBitacoraRepositorio _bitacoraRepositorio;
        private readonly ICriptografiaServicio _criptografia;
        private readonly int _minutosSesion;

        public AutenticacionServicio(string connectionString, string claveAes, int minutosSesion)
            : this(new UsuarioRepositorio(connectionString),
                   new BitacoraRepositorio(connectionString),
                   new CriptografiaServicio(claveAes),
                   minutosSesion)
        {
        }

        public AutenticacionServicio(
            IUsuarioRepositorio usuarioRepositorio,
            IBitacoraRepositorio bitacoraRepositorio,
            ICriptografiaServicio criptografia,
            int minutosSesion)
        {
            if (usuarioRepositorio == null) throw new ArgumentNullException(nameof(usuarioRepositorio));
            if (bitacoraRepositorio == null) throw new ArgumentNullException(nameof(bitacoraRepositorio));
            if (criptografia == null) throw new ArgumentNullException(nameof(criptografia));

            _usuarioRepositorio = usuarioRepositorio;
            _bitacoraRepositorio = bitacoraRepositorio;
            _criptografia = criptografia;
            _minutosSesion = minutosSesion > 0 ? minutosSesion : 60;
        }

        public ResultadoAutenticacion Autenticar(CredencialesUsuario credenciales)
        {
            // 1. Campos vacios: el criterio pide el MISMO mensaje que credenciales invalidas.
            if (credenciales == null ||
                string.IsNullOrWhiteSpace(credenciales.Usuario) ||
                string.IsNullOrWhiteSpace(credenciales.Password))
            {
                return ResultadoAutenticacion.Fallido(MsgCredenciales);
            }

            var nombreUsuario = credenciales.Usuario.Trim();

            try
            {
                // 2. Se busca sin filtrar por estado, para distinguir en la bitacora
                //    "no existe" de "existe pero esta bloqueado/inactivo".
                var usuario = _usuarioRepositorio.ObtenerPorUsuario(nombreUsuario);

                if (usuario == null)
                {
                    Bitacora("ERROR", string.Format("Login fallido: el usuario '{0}' no existe.", nombreUsuario), null);
                    return ResultadoAutenticacion.Fallido(MsgCredenciales);
                }

                // 3. Usuario no habilitado.
                if (!string.Equals(usuario.Estado, EstadoActivo, StringComparison.OrdinalIgnoreCase))
                {
                    Bitacora("ERROR",
                        string.Format("Login rechazado: el usuario '{0}' esta en estado {1}.", usuario.Usuario, usuario.Estado),
                        usuario.IdUsuario);
                    return ResultadoAutenticacion.Fallido(MsgCredenciales);
                }

                // 4. Verificacion de la contrasena con el mismo esquema AES-GCM del Alcance 1.
                if (!_criptografia.Verificar(credenciales.Password, usuario.PasswordHash))
                {
                    RegistrarIntentoFallido(usuario);
                    return ResultadoAutenticacion.Fallido(MsgCredenciales);
                }

                // 5. Credenciales validas: resetear intentos y registrar el ultimo ingreso.
                _usuarioRepositorio.RegistrarLoginExitoso(usuario.IdUsuario);

                var expira = DateTime.UtcNow.AddMinutes(_minutosSesion);

                Bitacora("SELECT",
                    string.Format("Login exitoso para el usuario '{0}'.", usuario.Usuario),
                    usuario.IdUsuario);

                return new ResultadoAutenticacion
                {
                    Autenticado = true,
                    Mensaje = "OK",
                    IdUsuario = usuario.IdUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    Token = GenerarToken(),
                    Expira = expira
                };
            }
            catch (Exception ex)
            {
                Bitacora("ERROR", string.Format("Error tecnico en el login: {0}", ex.Message), null);
                return ResultadoAutenticacion.Fallido("Ocurrió un error al procesar la autenticación.");
            }
        }

        private void RegistrarIntentoFallido(UsuarioAutenticacion usuario)
        {
            var intentos = usuario.IntentosLogin + 1;

            if (intentos >= MaxIntentos)
            {
                _usuarioRepositorio.Bloquear(usuario.IdUsuario, intentos);

                _bitacoraRepositorio.Registrar(
                    "UPDATE",
                    "usuario",
                    string.Format("Usuario '{0}' bloqueado por {1} intentos de login fallidos.", usuario.Usuario, MaxIntentos),
                    usuario.IdUsuario,
                    Json(new { estado = usuario.Estado, intentos_login = usuario.IntentosLogin }),
                    Json(new { estado = EstadoBloqueado, intentos_login = intentos }));

                return;
            }

            _usuarioRepositorio.ActualizarIntentos(usuario.IdUsuario, intentos);

            Bitacora("ERROR",
                string.Format("Login fallido para '{0}'. Intento {1} de {2}.", usuario.Usuario, intentos, MaxIntentos),
                usuario.IdUsuario);
        }

        private void Bitacora(string tipo, string descripcion, int? idUsuario)
        {
            _bitacoraRepositorio.Registrar(tipo, "usuario", descripcion, idUsuario, null, null);
        }

        private static string Json(object valor)
        {
            return valor == null ? null : new JavaScriptSerializer().Serialize(valor);
        }

        /// <summary>
        /// Token opaco de sesion. El Alcance 2 solo lo usa como marca de "hay sesion
        /// iniciada"; si mas adelante se requiere un JWT firmado, basta con cambiar
        /// esta implementacion sin tocar el contrato del servicio.
        /// </summary>
        private static string GenerarToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
    }
}
