using Google.Protobuf.Collections;
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
                foreach (var seguido in user.ColSeguidos)
                {
                    usuario.ColSeguidos.Add(CrearUsuario(seguido));
                }
                foreach (var seguidor in user.ColSeguidores)
                {
                    usuario.ColSeguidores.Add(CrearUsuario(seguidor));
                }
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
            usuario.ColPublicacion = CrearPublicacionesDeUsuario(user.Chips);
            usuario.ColNotif = CrearPublicacionesDeUsuario(user.ColNotif);
            return usuario;
        }

        public List<Publicacion> CrearPublicacionesDeUsuario(RepeatedField<Chip> chips)
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
                publicacion.ColRespuesta.AddRange(CrearRespuestasDeChip(chip.ColRespuesta));
                publicaciones.Add(publicacion);
            }
            return publicaciones;
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
    }
}
