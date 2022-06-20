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
                Id = Guid.NewGuid(),
                NombreUsuario = NombreUsuario,
                pContenido = Contenido,
                pFch = DateTime.Now,
                colRespuesta = new List<Respuesta>(),
                colFile = new List<string>()
            };
            return publicacion;
        }
    }
}
