using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor
{
    public class Publicacion
    {
        private DateTime pFch;
        private string pContenido;
        private List<Publicacion> ColRespuesta;
        private List<string> colFile;

        public Publicacion(string _contenido)
        {
            pFch = DateTime.Now;
            pContenido = _contenido.Trim();
            ColRespuesta = new List<Publicacion>();
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
