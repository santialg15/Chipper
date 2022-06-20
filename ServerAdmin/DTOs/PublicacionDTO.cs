using Logica;

namespace ServerAdmin.DTOs
{
    public class PublicacionDTO
    {
        public string NombreUsuario { get; set; }
        public string Contenido { get; set; }

        public Publicacion CrearPublicacion()
        {
            var publicacion = new Publicacion()
            {
                NombreUsuario = NombreUsuario,
                pContenido = Contenido,
            };
            return publicacion;
        }
    }
}
