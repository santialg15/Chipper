using Grpc.Net.Client;
using Logica;
using ServerAdminLogicDataAccessInterface;
using ServerAdminLogicInterface;

namespace ServerAdminLogic
{
    public class LogicaUsuario : ILogicaUsuario
    {
        private IUsersRepository userRepository;

        public LogicaUsuario(IUsersRepository usrRepository)
        {
            userRepository = usrRepository;
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

        public async Task GetAll()
        {
            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply =  await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
            var prueba = reply;
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            //
            throw new Exception();//return userRepository.GetAll();
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
