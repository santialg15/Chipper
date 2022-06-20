using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Logica;
using ServerAdminLogicDataAccessInterface;
using ServerAdminLogicInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAdminLogic
{
    public class LogicaPublicaciones : ILogicaPublicaciones
    {
        private readonly IUsersRepository userRepository;
        private readonly IPublicacionesRepository chipsRepository;
        private readonly Mapper mapper;

        public LogicaPublicaciones(IPublicacionesRepository chipsRepo, IUsersRepository usersRepo)
        {
            userRepository = usersRepo;
            chipsRepository = chipsRepo;
            mapper = new Mapper();
        }

        public async Task Delete(Guid idChip)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new DeleteChipRequest()
            {
                Id = idChip.ToString()
            };
            var reply = await client.DeleteChipAsync(request);
        }

        public async Task<Publicacion> GetById(Guid idChip)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new GetChipRequest();
            request.GuidId = idChip.ToString();
            var reply = await client.GetChipAsync(request);
            return mapper.CrearPublicacion(reply.Chip);
        }

        public async Task<List<Publicacion>> GetAll()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new GetChipsRequest();
            var reply = await client.GetChipsAsync(request);
            return mapper.CrearPublicaciones(reply.Chips);
        }

        public async Task<string> Insert(Publicacion publicacion)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new PostChipRequest()
            {
                Chip = mapper.CrearChip(publicacion)
            };
            var reply = await client.PostChipAsync(request);
            return reply.Response;
        }

        public async Task<string> Update(Publicacion publicacion)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new PutChipRequest()
            {
                Chip = mapper.CrearChip(publicacion)
            };
            var reply = await client.PutChipAsync(request);
            return reply.Response;
        }

        public async Task<string> CreateAnswer(Guid idPublicacion, Respuesta respuesta)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new PostAnswerRequest()
            {
                IdPublicacion = idPublicacion.ToString(),
                Answer = mapper.CrearAnswer(respuesta)
            };
            var reply = await client.PostAnswerAsync(request);
            return reply.Response;
        }

        public async void DeleteAnswer(Guid idPublicacion, Guid idRespuesta)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new DeleteAnswerRequest()
            {
                IdPublicacion = idPublicacion.ToString(),
                IdRespuesta = idRespuesta.ToString()
            };
            var reply = await client.DeleteAnswerAsync(request);
        }

        public Task<Publicacion> GetById(string key)
        {
            throw new NotImplementedException();
        }
        public Task Delete(string name)
        {
            throw new NotImplementedException();
        }
    }
}
