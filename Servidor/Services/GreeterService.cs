using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Logica;
using Microsoft.Extensions.Logging;

namespace Servidor.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly Mapper mapper;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
            mapper = new Mapper();
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
            List<Usuario> usuarios = Servidor.RetornarUsuarios();

            GetUsersReply reply = new GetUsersReply();
            reply.Users.AddRange(mapper.CreateUsers(usuarios));
            return Task.FromResult(reply);
        }

        public override Task<PostUserReply> PostUser(PostUserRequest request, ServerCallContext context)
        {
            PostUserReply reply = new PostUserReply();
            reply.Response = Servidor.CrearUsuario(mapper.CreateUsuario(request.User));
            return Task.FromResult(reply);
        }

    }
}

