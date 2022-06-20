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

        public async Task Delete(string name)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new DeleteUserRequest()
            {
                PNomUsu = name
            };
            var reply = await client.DeleteUserAsync(request);
            //Usuario usuarioABorrar = GetById(idUsuario);
            //if (usuarioABorrar == null)
            //    throw new NullReferenceException("El usuario a borrar no existe.");
            //userRepository.Delete(usuarioABorrar);
            //throw new Exception(); 
        }

        public async Task<Usuario> GetById(string name)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new GetUserRequest()
            {
                PNomUsu = name
            };
            var reply = await client.GetUserAsync(request);
            var usuario = mapper.CrearUsuario(reply.User);
            return usuario;
            //var usuarioAObtener = userRepository.GetById(name);
            //if(usuarioAObtener == null)
            //    throw new NullReferenceException("El usuario a obtener no existe.");
            //return usuarioAObtener;
        }

        public async Task<List<Usuario>> GetAll()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new GetUsersRequest();
            var reply = await client.GetUsersAsync(request);
            var usuarios = mapper.CrearUsuarios(reply);
            return usuarios;
            //return userRepository.GetAll();
        }

        public async Task<string> Insert(Usuario usuario)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new PostUserRequest()
            {
                User = mapper.CrearUser(usuario)
            };
            var reply = await client.PostUserAsync(request);
            return reply.Response;
            //if (userRepository.Exist(usuario.PNomUsu))
            //    throw new ArgumentException("Ya existe un usuario con ese nombre.");
            //return userRepository.Insert(usuario);
        }

        public bool Exist(string nombreUsuario)
        {
            return userRepository.Exist(nombreUsuario);
        }

        public async Task<string> Update(Usuario usuario)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new PutUserRequest()
            {
                User = mapper.CrearUser(usuario)
            };
            var reply = await client.PutUserAsync(request);
            return reply.Response;
            //var usuarioAModificar = GetById(usuario.Id);
            //if (usuarioAModificar == null)
            //    throw new NullReferenceException("El usuario a modificar no existe.");
            //else
            //{
            //    if(usuario.PNomUsu != usuarioAModificar.PNomUsu)
            //        throw new ArgumentException("El nombre del usuario no puede ser modificado.");
            //}
            //return userRepository.Update(usuario);
        }
    }
}
