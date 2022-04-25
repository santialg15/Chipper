
namespace Servidor
{
    public class Respuesta
    {
        public string pNomUsu;
        public DateTime pFch;
        public string pContenido;

        public Respuesta(string nombreUsuario, string contenido)
        {
            pNomUsu = nombreUsuario;
            pFch = DateTime.Now;
            pContenido = contenido;
        }

        public string PNomUsu { get => pNomUsu; set => pNomUsu = value;}
        public DateTime PFch { get => pFch; set => pFch = value; }
        public string PContenido { get => pContenido; set => pContenido = value; }

        public override string ToString()
        {
            return $"{pNomUsu}:{pFch} -> {pContenido}";
        }
    }
}
