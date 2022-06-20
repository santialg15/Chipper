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
            var reply = CreateGetUsersReply(usuarios);
            return Task.FromResult(reply);
        }

        private static GetUsersReply CreateGetUsersReply(List<Usuario> usuarios)
        {
            GetUsersReply getUsersReply = new GetUsersReply();
            var users = getUsersReply.Users;
            users.AddRange(CreateUsers(usuarios));
            return getUsersReply;
        }

        private static RepeatedField<User> CreateUsers(List<Usuario> usuarios)
        {
            RepeatedField<User> users = new RepeatedField<User>();
            foreach (var usuario in usuarios)
            {
                User user = CreateUser(usuario);
                foreach(var seguido in usuario.ColSeguidos)
                {
                    user.ColSeguidos.Add(CreateUser(seguido));
                }
                user.Chips.AddRange(CreateChipsOfUser(usuario.ColPublicacion));
                users.Add(user);
            }
            return users;
        }

        private static User CreateUser(Usuario usuario)
        {
            User user = new User()
            {
                PNomUsu = usuario.PNomUsu,
                PNomReal = usuario.PNomReal,
                Pass = usuario.Pass,
                EstaLogueado = usuario.estaLogueado,
                Habilitado = usuario.Habilitado
            };
            return user;
        }


        private static RepeatedField<Chip> CreateChipsOfUser(List<Publicacion> publicaciones)
        {
            RepeatedField<Chip> chips = new RepeatedField<Chip>();
            foreach (var publicacion in publicaciones)
            {
                Chip chip = new Chip()
                {
                    Id = publicacion.id,
                    PFch = Timestamp.FromDateTime(publicacion.PFch),
                    PContenido = publicacion.Contenido,
                };
                chip.ColRespuesta.AddRange(CreateAnswersOfChips(publicacion.ColRespuesta));
                chips.Add(chip);
            }
            return chips;
        }

        private static RepeatedField<Answer> CreateAnswersOfChips(List<Respuesta> respuestas)
        {
            RepeatedField<Answer> answers = new RepeatedField<Answer>();
            foreach (var respuesta in respuestas)
            {
                Answer answer = new Answer()
                {
                    PNomUsu = respuesta.PNomUsu,
                    PFch = Timestamp.FromDateTime(respuesta.PFch),
                    PContenido = respuesta.PContenido
                };
                answers.Add(answer);
            }
            return answers;
        }
    }
}

