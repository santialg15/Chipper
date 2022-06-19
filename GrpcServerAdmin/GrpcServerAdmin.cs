using Logica;
using ServerAdminGrpcInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServerAdmin
{
    public class GrpcServerAdmin : IGrpcServerAdmin
    {
        public void Bloquear(int idUsuario)
        {
            throw new NotImplementedException();
        }

        public void Desbloquear(int idUsuario)
        {
            throw new NotImplementedException();
        }

        public Usuario Insertar(Usuario usuario)
        {
            throw new NotImplementedException();
        }
    }
}
