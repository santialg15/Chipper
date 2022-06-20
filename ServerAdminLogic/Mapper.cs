using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Logica;

namespace ServerAdminLogic
{
    public class Mapper
    {
        public List<Usuario> CrearUsuarios(GetUsersReply usersReply)
        {
            List<Usuario> usuarios = new List<Usuario>();
            foreach (var user in usersReply.Users)
            {
                Usuario usuario = CrearUsuario(user);
                usuarios.Add(usuario);
            }
            return usuarios;
        }

        public Usuario CrearUsuario(User user)
        {
            Usuario usuario = new Usuario()
            {
                PNomUsu = user.PNomUsu,
                PNomReal = user.PNomReal,
                Pass = user.Pass,
                estaLogueado = user.EstaLogueado,
                Habilitado = user.Habilitado,
            };
            foreach (var seguido in user.ColSeguidos)
            {
                Usuario usuarioSeguido = new Usuario()
                {
                    PNomUsu = seguido.PNomUsu,
                    PNomReal = seguido.PNomReal,
                };
                usuario.ColSeguidos.Add(usuarioSeguido);
            }
            foreach (var seguidor in user.ColSeguidores)
            {
                Usuario usuarioSeguidor = new Usuario()
                {
                    PNomUsu = seguidor.PNomUsu,
                    PNomReal = seguidor.PNomReal,
                };
                usuario.ColSeguidores.Add(usuarioSeguidor);
            }
            usuario.ColPublicacion = CrearPublicaciones(user.Chips);
            usuario.ColNotif = CrearPublicaciones(user.ColNotif);
            return usuario;
        }

        public List<Publicacion> CrearPublicaciones(RepeatedField<Chip> chips)
        {
            List<Publicacion> publicaciones = new List<Publicacion>();
            foreach (var chip in chips)
            {
                Publicacion publicacion = CrearPublicacion(chip);
                publicaciones.Add(publicacion);
            }
            return publicaciones;
        }

        public Publicacion CrearPublicacion(Chip chip)
        {
            Publicacion publicacion = new Publicacion()
            {
                Id = Guid.Parse(chip.Id),
                NombreUsuario = chip.UserName,
                PFch = chip.PFch.ToDateTime(),
                Contenido = chip.PContenido,
            };
            publicacion.ColRespuesta.AddRange(CrearRespuestasDeChip(chip.ColRespuesta));
            return publicacion;
        }

        public List<Respuesta> CrearRespuestasDeChip(RepeatedField<Answer> answers)
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

        public User CrearUser(Usuario usuario)
        {
            User user = new User()
            {
                PNomUsu = usuario.PNomUsu,
                PNomReal = usuario.PNomReal,
                Pass = usuario.Pass,
                Habilitado = true,
            };
            return user;
        }

        public Chip CrearChip(Publicacion publicacion)
        {
            Chip chip = new Chip()
            {
                Id = publicacion.Id.ToString(),
                UserName = publicacion.NombreUsuario,
                PFch = Timestamp.FromDateTime(publicacion.PFch),
                PContenido = publicacion.Contenido,
            };
            chip.ColRespuesta.AddRange(CrearAnswersOfChips(publicacion.ColRespuesta));
            return chip;
        }

        public RepeatedField<Answer> CrearAnswersOfChips(List<Respuesta> respuestas)
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
