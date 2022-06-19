using Logica;

namespace ServerAdmin.DTOs
{
    public class UsuarioDTO
    {
        public string PNomUsu { get; set; }
        public string PNomReal { get; set; }
        public string Pass { get; set; }

        public Usuario CrearUsuario()
        {
            Usuario usuarioARetornar = new Usuario()
            {
                PNomUsu = this.PNomUsu,
                PNomReal = this.PNomReal,
                Pass = this.Pass,
                Habilitado = true,
                ColPublicacion = new List<Publicacion>(),
                ColSeguidos = new List<Usuario>(),
                ColSeguidores = new List<Usuario>(),
                ColNotif = new List<Publicacion>(),
            };
            return usuarioARetornar;
        }
    }
}
