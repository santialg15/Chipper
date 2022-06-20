using Grpc.Net.Client;
using Logica;
using ServerAdminLogicInterface;

namespace ServerAdminLogic
{
    public class LogicaPublicaciones : ILogicaPublicaciones
    {
        private readonly Mapper mapper;
        GrpcChannel channel;
        Greeter.GreeterClient client;

        public LogicaPublicaciones()
        {
            mapper = new Mapper();
            channel = GrpcChannel.ForAddress("https://localhost:5001");
            client = new Greeter.GreeterClient(channel);
        }

        public async Task Delete(Guid idChip)
        {
            var request = new DeleteChipRequest()
            {
                Id = idChip.ToString()
            };
            var reply = await client.DeleteChipAsync(request);
        }

        public async Task<Publicacion> GetById(Guid idChip)
        {
            var request = new GetChipRequest();
            request.GuidId = idChip.ToString();
            var reply = await client.GetChipAsync(request);
            return mapper.CrearPublicacion(reply.Chip);
        }

        public async Task<List<Publicacion>> GetAll()
        {
            var request = new GetChipsRequest();
            var reply = await client.GetChipsAsync(request);
            return mapper.CrearPublicaciones(reply.Chips);
        }

        public async Task<string> Insert(Publicacion publicacion)
        {
            var request = new PostChipRequest()
            {
                Chip = mapper.CrearChip(publicacion)
            };
            var reply = await client.PostChipAsync(request);
            return reply.Response;
        }

        public async Task<string> Update(Publicacion publicacion)
        {
            var request = new PutChipRequest()
            {
                Chip = mapper.CrearChip(publicacion)
            };
            var reply = await client.PutChipAsync(request);
            return reply.Response;
        }

        public async Task<string> CreateAnswer(Guid idPublicacion, Respuesta respuesta)
        {
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
