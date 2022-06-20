using Grpc.Net.Client;
using Logica;
using ServerAdminLogicInterface;

namespace ServerAdminLogic
{
    public class LogicaUsuario : ILogicaUsuario 
    {
        private readonly Mapper mapper;
        GrpcChannel channel;
        Greeter.GreeterClient client;

        public LogicaUsuario()
        {
            mapper = new Mapper();
            channel = GrpcChannel.ForAddress("https://localhost:5001");
            client = new Greeter.GreeterClient(channel);
        }

        public async Task Delete(string name)
        {
            var request = new DeleteUserRequest()
            {
                PNomUsu = name
            };
            var reply = await client.DeleteUserAsync(request); 
        }

        public async Task<Usuario> GetById(string name)
        {
            var request = new GetUserRequest()
            {
                PNomUsu = name
            };
            var reply = await client.GetUserAsync(request);
            var usuario = mapper.CrearUsuario(reply.User);
            return usuario;
        }

        public async Task<List<Usuario>> GetAll()
        {
            var request = new GetUsersRequest();
            var reply = await client.GetUsersAsync(request);
            var usuarios = mapper.CrearUsuarios(reply);
            return usuarios;
        }

        public async Task<string> Insert(Usuario usuario)
        {
            var request = new PostUserRequest()
            {
                User = mapper.CrearUser(usuario)
            };
            var reply = await client.PostUserAsync(request);
            return reply.Response;
        }

        public async Task<string> Update(Usuario usuario)
        {
            var request = new PutUserRequest()
            {
                User = mapper.CrearUser(usuario)
            };
            var reply = await client.PutUserAsync(request);
            return reply.Response;
        }

        public async Task ChangePermission(string name)
        {
            var request = new ChangeEnableUserRequest()
            {
                UserName = name
            };
            var reply = await client.ChangeEnableUserAsync(request);
        }
    }
}
