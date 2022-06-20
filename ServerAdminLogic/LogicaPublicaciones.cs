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

        public Task Delete(string idChip)
        {
            throw new Exception();
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

        public Task<string> Update(Publicacion chip)
        {

            throw new Exception();
        }

        public Respuesta CreateAnswer(Guid idPublicacion, Respuesta respuesta)
        {
            var chip = chipsRepository.GetById(idPublicacion);
            if (chip == null)
                throw new NullReferenceException("El chip a responder no existe.");
            chipsRepository.AddAnswer(idPublicacion, respuesta);
            return respuesta;
        }

        public void DeleteAnswer(Guid idPublicacion, Guid idRespuesta)
        {
            var chip = chipsRepository.GetById(idPublicacion);
            if (chip == null)
                throw new NullReferenceException("El chip a de la respuesta a borrar no existe.");
            var respuesta = chip.ColRespuesta.FirstOrDefault(r => r.Id == idRespuesta);
            if (respuesta == null)
                throw new NullReferenceException("La respuesta del chip no existe.");
            chipsRepository.DeleteAnswer(idPublicacion, respuesta);
        }

        public Task<Publicacion> GetById(string key)
        {
            throw new NotImplementedException();
        }
    }
}
