
using System.Reflection.Metadata.Ecma335;

namespace Logica
{
    public class Publicacion
    {
        public int id;
        public DateTime pFch;
        public string pContenido;
        public List<Respuesta> colRespuesta;
        public List<string> colFile;

        public Guid IdUsuario { get; set; }
        public Guid Id { get; set; }
        public int IdP { get; set; }
        public DateTime PFch { get => pFch; set => pFch = value; }
        public string Contenido { get => pContenido; set => pContenido = value; }
        public List<Respuesta> ColRespuesta { get => colRespuesta; set => colRespuesta = value; }

        public Publicacion()
        {
            ColRespuesta = new List<Respuesta>();
        }

        public Publicacion(string _contenido, int idPublicacion)
        {
            id = idPublicacion+1;
            pFch = DateTime.UtcNow;
            pContenido = _contenido.Trim();
            colRespuesta = new List<Respuesta>();
            colFile = new List<string>();
        }

        public override string ToString()
        {
            String ret = $"{id} -> Fecha: {pFch} | Contenido: {pContenido.Trim()}";
            if (colFile.Count > 0)
            {
                ret += " | ";
                for (int i = 0; i < colFile.Count; i++)
                {
                    ret += "Imagen " + (i + 1).ToString() +": "+ colFile[i].Trim()+" ";
                }
            }

            return ret;
        }

        public string getContenido()
        {
            return pContenido;
        }

        public DateTime getFch()
        {
            return pFch;
        }

        public void addFile(string img)
        {
            colFile.Add(img);
        }
    }
}
