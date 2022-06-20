using Logica;

namespace ServerAdmin.DTOs
{
    public class RespuestaDTO
    {
        public string NombreUsuario { get; set; }
        public string Contenido { get; set; }

        public Respuesta CrearRespuesta()
        {
            Respuesta respuesta = new Respuesta()
            {
                PContenido = Contenido,
                PNomUsu = NombreUsuario,
            };
            return respuesta;
        }
    }
}
