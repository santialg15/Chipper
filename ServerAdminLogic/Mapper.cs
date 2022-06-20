using Google.Protobuf.Collections;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAdminLogic
{
    public class Mapper
    {
        public List<Usuario> CreateUsers(GetUsersReply usersReply)
        {
            List<Usuario> usuarios = new List<Usuario>();
            foreach (var user in usersReply.Users)
            {
                Usuario usuario = CreateUser(user);
                foreach (var seguido in user.ColSeguidos)
                {
                    usuario.ColSeguidos.Add(CreateUser(seguido));
                }
                foreach (var seguidor in user.ColSeguidores)
                {
                    usuario.ColSeguidores.Add(CreateUser(seguidor));
                }
                usuarios.Add(usuario);
            }
            return usuarios;
        }

        public Usuario CreateUser(User user)
        {
            Usuario usuario = new Usuario()
            {
                PNomUsu = user.PNomUsu,
                PNomReal = user.PNomReal,
                Pass = user.Pass,
                estaLogueado = user.EstaLogueado,
                Habilitado = user.Habilitado,
            };
            usuario.ColPublicacion = CreateChipsOfUser(user.Chips);
            usuario.ColNotif = CreateChipsOfUser(user.ColNotif);
            return usuario;
        }

        public List<Publicacion> CreateChipsOfUser(RepeatedField<Chip> chips)
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

        public List<Respuesta> CreateAnswersOfChips(RepeatedField<Answer> answers)
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
