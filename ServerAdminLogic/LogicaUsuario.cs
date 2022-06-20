using System.Linq.Expressions;
using Google.Protobuf.Collections;
using Grpc.Net.Client;
using Logica;
using ServerAdminLogicDataAccessInterface;
using ServerAdminLogicInterface;

namespace ServerAdminLogic
{
    public class LogicaUsuario : ILogicaUsuario
    {
        private IUsersRepository userRepository;
        private readonly Mapper mapper;

        public LogicaUsuario(IUsersRepository usrRepository)
        {
            userRepository = usrRepository;
            mapper = new Mapper();
        }

        public void Delete(Guid idUsuario)
        {
            Usuario usuarioABorrar = GetById(idUsuario);
            if (usuarioABorrar == null)
                throw new NullReferenceException("El usuario a borrar no existe.");
            userRepository.Delete(usuarioABorrar);
        }

        public Usuario GetById(Guid idUsuario)
        {
            var usuarioAObtener = userRepository.GetById(idUsuario);
            if(usuarioAObtener == null)
                throw new NullReferenceException("El usuario a obtener no existe.");
            return usuarioAObtener;
        }

        public async Task<List<Usuario>> GetAll()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new GetUsersRequest();
            var reply = await client.GetUsersAsync(request);
            var usuarios = mapper.CreateUsers(reply);
            return usuarios;
            //return userRepository.GetAll();
        }

        public Usuario Insert(Usuario usuario)
        {
            if (userRepository.Exist(usuario.PNomUsu))
                throw new ArgumentException("Ya existe un usuario con ese nombre.");
            return userRepository.Insert(usuario);
        }

        public bool Exist(string nombreUsuario)
        {
            return userRepository.Exist(nombreUsuario);
        }

        public Usuario Update(Usuario usuario)
        {
            var usuarioAModificar = GetById(usuario.Id);
            if (usuarioAModificar == null)
                throw new NullReferenceException("El usuario a modificar no existe.");
            else
            {
                if(usuario.PNomUsu != usuarioAModificar.PNomUsu)
                    throw new ArgumentException("El nombre del usuario no puede ser modificado.");
            }
            return userRepository.Update(usuario);
        }
    }
}
