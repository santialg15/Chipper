using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IUsuario
    {
        List<Usuario> ObtenerTodos();
        Usuario Obtener(int idUsuario);
        Usuario Insertar(Usuario usuario);
        Usuario Modificar(Usuario usuario);
        void Eliminar(int idUsuario);
    }
}
