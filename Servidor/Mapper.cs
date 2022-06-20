using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor
{
    public class Mapper
    {
        public RepeatedField<User> CreateUsers(List<Usuario> usuarios)
        {
            RepeatedField<User> users = new RepeatedField<User>();
            foreach (var usuario in usuarios)
            {
                User user = CreateUser(usuario);
                foreach (var seguido in usuario.ColSeguidos)
                {
                    user.ColSeguidos.Add(CreateUser(seguido));
                }
                foreach (var seguidor in usuario.ColSeguidores)
                {
                    user.ColSeguidores.Add(CreateUser(seguidor));
                }
                users.Add(user);
            }
            return users;
        }

        public User CreateUser(Usuario usuario)
        {
            User user = new User()
            {
                PNomUsu = usuario.PNomUsu,
                PNomReal = usuario.PNomReal,
                Pass = usuario.Pass,
                EstaLogueado = usuario.estaLogueado,
                Habilitado = usuario.Habilitado
            };
            user.Chips.AddRange(CreateChipsOfUser(usuario.ColPublicacion));
            user.ColNotif.AddRange(CreateChipsOfUser(usuario.ColNotif));
            return user;
        }

        public RepeatedField<Chip> CreateChipsOfUser(List<Publicacion> publicaciones)
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

        public RepeatedField<Answer> CreateAnswersOfChips(List<Respuesta> respuestas)
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

        public Usuario CreateUsuario(User user)
        {
            Usuario usuario = new Usuario()
            {
                PNomUsu = user.PNomUsu,
                PNomReal = user.PNomReal,
                Pass = user.Pass
            };
            return usuario;
        }
    }
}
