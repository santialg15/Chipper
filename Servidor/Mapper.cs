﻿using Google.Protobuf.Collections;
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
                //foreach (var seguido in usuario.ColSeguidos)
                //{
                //    user.ColSeguidos.Add(CreateUser(seguido));
                //}
                //foreach (var seguidor in usuario.ColSeguidores)
                //{
                //    user.ColSeguidores.Add(CreateUser(seguidor));
                //}
                users.Add(user);
            }
            return users;
        }

        public User CreateUser(Usuario usuario)
        {
            if(usuario == null)
                throw new NullReferenceException("El usuario a obtener no existe.");
            User user = new User()
            {
                PNomUsu = usuario.PNomUsu,
                PNomReal = usuario.PNomReal,
                Pass = usuario.Pass,
                EstaLogueado = usuario.estaLogueado,
                Habilitado = usuario.Habilitado
            };
            foreach (var seguido in usuario.ColSeguidos)
            {
                User userSeguido = new User()
                {
                    PNomUsu = seguido.PNomUsu,
                    PNomReal = seguido.PNomReal,
                    //Pass = seguido.Pass,
                    //EstaLogueado = seguido.estaLogueado,
                    //Habilitado = seguido.Habilitado
                };
                user.ColSeguidos.Add(userSeguido);
            }
            foreach (var seguidor in usuario.ColSeguidores)
            {
                User userSeguidor = new User()
                {
                    PNomUsu = seguidor.PNomUsu,
                    PNomReal = seguidor.PNomReal,
                    //Pass = seguidor.Pass,
                    //EstaLogueado = seguido.estaLogueado,
                    //Habilitado = seguido.Habilitado
                };
                user.ColSeguidores.Add(userSeguidor);
            }
            user.Chips.AddRange(CreateChips(usuario.ColPublicacion));
            user.ColNotif.AddRange(CreateChips(usuario.ColNotif));
            return user;
        }

        public RepeatedField<Chip> CreateChips(List<Publicacion> publicaciones)
        {
            RepeatedField<Chip> chips = new RepeatedField<Chip>();
            foreach (var publicacion in publicaciones)
            {
                Chip chip = CreateChip(publicacion);
                //chip.ColRespuesta.AddRange(CreateAnswersOfChips(publicacion.ColRespuesta));
                chips.Add(chip);
            }
            return chips;
        }

        public Chip CreateChip(Publicacion publicacion)
        {
            Chip chip = new Chip()
            {
                Id = publicacion.Id.ToString(),
                UserName = publicacion.NombreUsuario,
                PFch = Timestamp.FromDateTime(publicacion.PFch),
                PContenido = publicacion.Contenido,
            };
            chip.ColRespuesta.AddRange(CreateAnswersOfChips(publicacion.ColRespuesta));
            return chip;
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
