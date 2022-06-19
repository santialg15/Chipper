using Grpc.Core;
using Logica;
using Microsoft.Extensions.Logging;

namespace Servidor.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<GetUsersReply> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            List<Usuario> usuarios = Servidor.ReturnUsers();
            var reply = CreateUser(usuarios);//new GetUsersReply();
            //reply.Users.AddRange((IEnumerable<User>)usuarios);
            return Task.FromResult(reply);
        }

        private static GetUsersReply CreateUser(List<Usuario> usuarios)
        {
            GetUsersReply getUsersReply = new GetUsersReply();
            var users = getUsersReply.Users;
            foreach (var usuario in usuarios)
            {
                users.Add(new User
                {
                    PNomUsu = usuario.PNomUsu,
                    PNomReal = usuario.PNomReal,
                    Pass = usuario.Pass,
                    ImgPerfil = "",
                    EstaLogueado = usuario.estaLogueado,
                    Habilitado = usuario.Habilitado
                });
            }
            return getUsersReply;
        }

        //public override Task<PostUserReply> PostUser(PostUserRequest request, ServerCallContext context)
        //{
        //    return Task.FromResult(new PostUserReply { User = request.User });
        //}
    }
}
