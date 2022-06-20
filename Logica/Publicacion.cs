﻿
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

        public string NombreUsuario { get; set; }
        public Guid Id { get; set; }
        public DateTime PFch { get => pFch; set => pFch = value; }
        public string Contenido { get => pContenido; set => pContenido = value; }
        public List<Respuesta> ColRespuesta { get => colRespuesta; set => colRespuesta = value; }

        public Publicacion()
        {
            Id = Guid.NewGuid();
            PFch = DateTime.UtcNow;
            ColRespuesta = new List<Respuesta>();
            colFile = new List<string>();
        }

        public Publicacion(string _contenido, int idPublicacion, string nombreUsuario)
        {
            id = idPublicacion+1;
            Id = Guid.NewGuid();
            PFch = DateTime.UtcNow;
            PFch = DateTime.UtcNow;
            pContenido = _contenido.Trim();
            colRespuesta = new List<Respuesta>();
            colFile = new List<string>();
            NombreUsuario = nombreUsuario;
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
