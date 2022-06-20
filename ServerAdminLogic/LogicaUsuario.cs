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

        public async Task<List<Usuario>> GetAll()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var request = new GetUsersRequest();
            var reply = await client.GetUsersAsync(request);
            var usuarios = CreateUsers(reply);
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

        private List<Usuario> CreateUsers(GetUsersReply usersReply)
        {
            List<Usuario> usuarios = new List<Usuario>();
            foreach (var user in usersReply.Users)
            {
                Usuario usuario = CreateUser(user);
                foreach (var seguido in user.ColSeguidos)
                {
                    usuario.ColSeguidos.Add(CreateUser(seguido));
                }
                usuario.ColPublicacion = CreateChipsOfUser(user.Chips);
                usuarios.Add(usuario);
            }
            return usuarios;
        }

        private static Usuario CreateUser(User user)
        {
            Usuario usuario = new Usuario()
            {
                PNomUsu = user.PNomUsu,
                PNomReal = user.PNomReal,
                Pass = user.Pass,
                estaLogueado = user.EstaLogueado,
                Habilitado = user.Habilitado,
                ColNotif = new List<Publicacion>(),
                ColPublicacion = new List<Publicacion>(),
                ColSeguidores = new List<Usuario>(),
            };
            return usuario;
        }

        private List<Publicacion> CreateChipsOfUser(RepeatedField<Chip> chips)
        {
            List<Publicacion> publicaciones = new List<Publicacion>();
            foreach (var chip in chips)
            {
                Publicacion publicacion = new Publicacion()
                {
                    IdP = chip.Id,
                    PFch = chip.PFch.ToDateTime(),
                    Contenido = chip.PContenido,
                };
                publicacion.ColRespuesta.AddRange(CreateAnswersOfChips(chip.ColRespuesta));
                publicaciones.Add(publicacion);
            }
            return publicaciones;
        }

        private List<Respuesta> CreateAnswersOfChips(RepeatedField<Answer> answers)
        {
            List<Respuesta> respuestas = new List<Respuesta>();
            foreach (var answer in answers)
            {
                Respuesta respuesta = new Respuesta()
                {
                    PNomUsu = answer.PNomUsu,
                    PFch = answer.PFch.ToDateTime(),
                    PContenido = answer.PContenido
                };
                respuestas.Add(respuesta);
            }
            return respuestas;
        }
    }
}
