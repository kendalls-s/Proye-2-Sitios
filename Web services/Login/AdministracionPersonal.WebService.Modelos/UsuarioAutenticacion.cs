namespace AdministracionPersonal.WebService.Modelos
{
    /// <summary>
    /// Proyeccion interna de la tabla usuario utilizada por las capas de
    /// acceso a datos y de logica de negocio. No es un DataContract: nunca
    /// se expone por el servicio porque contiene el password_hash.
    /// </summary>
    public class UsuarioAutenticacion
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string PasswordHash { get; set; }
        public string Estado { get; set; }
        public int IntentosLogin { get; set; }
    }
}
