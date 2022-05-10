namespace Logica
{
    public class Usuario
    {

        private const int TmpMostrarPub = 10;
        private string pNomReal;
        private string pNomUsu;
        private string pass;
        private string imgPerfil;
        public bool habilitado;
        private List<Usuario> colSeguidores;
        private List<Usuario> colSeguidos;
        private List<Publicacion> colPublicacion;
        private List<Publicacion> colNotif;

        public string PNomUsu { get => pNomUsu; }

        public string PNomReal { get => pNomReal; }
        public string Pass { get => pass; }

        public bool Habilitado { get => habilitado; set => habilitado = value; }

        public List<Usuario> getColSeguidores { get => colSeguidores; }

        public List<Publicacion> getColNotif { get => colNotif; }

        public List<Publicacion> ColPublicacion { get => colPublicacion; }

        public List<Usuario> ColSeguidores { get => colSeguidores; }

        public List<Usuario> ColSeguidos { get => colSeguidos; }


        public Usuario(string _NomReal, string _NomUsu, string _pass, string _imgPerfil)
        {
            pNomReal   = _NomReal;
            pNomUsu    = _NomUsu;
            pass       = _pass;
            imgPerfil  = _imgPerfil;
            habilitado = true;
            colSeguidores = new List<Usuario>();
            colSeguidos = new List<Usuario>();
            colPublicacion = new List<Publicacion>();
            colNotif = new List<Publicacion>();
        }

        public override string ToString()
        {
            return "Usuario: " +pNomUsu.Trim()+ " Nombre: "+ pNomReal.Trim()+ " Cantidad de Seguidores: "+ colSeguidores.Count.ToString().Trim() + " Sigue a: " + colSeguidos.Count.ToString().Trim() + " Cantidad de chips: " + colPublicacion.Count.ToString().Trim();
        }

        public List<Publicacion> GetPublicaciones()
        {
            return colPublicacion;
        }

        public string getNomUsu()
        {
            return pNomUsu;
        }

        public int GetCantSeg()
        {
            return colSeguidores.Count;
        }

        public int GetCantPubEnTmpConf()
        {
            int contador = 0;
            for (int i = 0; i < colPublicacion.Count; i++)
            {
                if ((DateTime.Now - colPublicacion[i].getFch()).TotalMinutes <= TmpMostrarPub)
                {
                    contador++;
                }
            }
            return contador;
        }


        public Publicacion nuevoChip(string chip)
        {
            Publicacion nuevaPub = new Publicacion(chip, colPublicacion.Count);
            colPublicacion.Add(nuevaPub);
            return nuevaPub;
        }

        public Publicacion nuevoChipConImg(string chip,string img)
        {
            Publicacion nuevaPub = new Publicacion(chip, colPublicacion.Count);
            var archivos = img.Split("?");
            foreach (var a in archivos)
            {
                nuevaPub.addFile(a);
            }
            colPublicacion.Add(nuevaPub);
            return nuevaPub;
        }

        public void AddNotif(Publicacion notif)
        {
            colNotif.Add(notif);
        }


        public void clearNotif()
        {
            colNotif.Clear();
        }
    }
}
