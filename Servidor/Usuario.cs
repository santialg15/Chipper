using System;
using System.Collections.Generic;
using System.Text;

namespace Servidor
{
    public class Usuario
    {
        private string pNom;
        private string pCi;

        public Usuario(string _Ci, string _nom)
        {
            pNom = _nom;
            pCi = _Ci;
        }

        public override string ToString()
        {
            return pNom +" "+pCi;
        }
    }
}
