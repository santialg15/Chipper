using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor
{
    public class Publicacion
    {
        public DateTime pFch;
        public string pContenido;
        public List<Respuesta> colRespuesta;
        public List<string> colFile;

        public Publicacion(string _contenido)
        {
            pFch = DateTime.Now;
            pContenido = _contenido.Trim();
            colRespuesta = new List<Respuesta>();
            colFile = new List<string>();
        }

        public string getContenido()
        {
            return pContenido;
        }

        public DateTime getFch()
        {
            return pFch;
        }
    }
}
