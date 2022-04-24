using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
