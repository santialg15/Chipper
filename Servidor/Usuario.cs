using System;
using System.Collections.Generic;
using System.Text;

namespace Servidor
{
    public class Usuario
    {
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
        public string Pass { get => pass; }

        public bool Habilitado { get => habilitado; set => habilitado = value; }


        public Usuario(string _NomReal, string _NomUsu, string _pass, string _imgPerfil)
        {
            pNomReal   = _NomReal;
            pNomUsu    = _NomUsu    ;
            pass       = _pass       ;
            imgPerfil  = _imgPerfil ;
            habilitado = true;
        }

        public override string ToString()
        {
            return pNomReal + " "+ pNomUsu;
        }
    }
}
