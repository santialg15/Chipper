using Logica;
using ServerAdminLogicDataAccessInterface;

namespace ServerAdminDataAccess
{
    public class UsersRepository : IUsersRepository
    {
        private List<Usuario> usuarios;
        public UsersRepository()
        {
            usuarios = new List<Usuario>();
        }

        public void Delete(Usuario usuario)
        {
            usuarios.Remove(usuario);
        }

        public bool Exist(string nombreUsuario)
        {
            return usuarios.Any(u => u.PNomUsu == nombreUsuario);
        }

        public IEnumerable<Usuario> GetAll()
        {
            return usuarios;
        }

        public Usuario GetById(Guid id)
        {
            return usuarios.FirstOrDefault(u => u.Id == id);
        }

        public Usuario Insert(Usuario usuario)
        {
            usuario.Id = Guid.NewGuid();
            usuarios.Add(usuario);
            return usuario;
        }

        public Usuario Update(Usuario usuario)
        {
            var usuarioAModificar = GetById(usuario.Id);
            usuarioAModificar = usuario;
            return usuario;
        }
    }
}