using Logica;

namespace ServerAdmin.DTOs
{
    public class PublicacionDTO
    {
        public Guid IdUsuario { get; set; }
        public string Contenido { get; set; }

        public Publicacion CrearPublicacion()
        {
            var publicacion = new Publicacion()
            {
                IdUsuario = IdUsuario,
                pContenido = Contenido,
                pFch = DateTime.Now,
                colRespuesta = new List<Respuesta>(),
                colFile = new List<string>()
            };
            return publicacion;
        }
    }
}
