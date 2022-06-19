using Logica;
using ServerAdminLogicDataAccessInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAdminDataAccess
{
    public class PublicacionesRepository : IPublicacionesRepository
    {
        private List<Publicacion> chips;

        public PublicacionesRepository()
        {
            chips = new List<Publicacion>();
        }

        public void AddAnswer(Guid idPublicacion, Respuesta respuesta)
        {
            var chip = GetById(idPublicacion);
            respuesta.Id = Guid.NewGuid();
            chip.ColRespuesta.Add(respuesta);
        }

        public void Delete(Publicacion chip)
        {
            chips.Remove(chip);
        }

        public void DeleteAnswer(Guid idPublicacion, Respuesta respuesta)
        {
            var chip = GetById(idPublicacion);
            chip.ColRespuesta.Remove(respuesta);
        }

        public IEnumerable<Publicacion> GetAll()
        {
            return chips;
        }

        public Publicacion GetById(Guid id)
        {
            return chips.FirstOrDefault(x => x.Id == id);
        }

        public Publicacion Insert(Publicacion chip)
        {
            chip.Id = Guid.NewGuid();
            chips.Add(chip);
            return chip;
        }

        public Publicacion Update(Publicacion chip)
        {
            var chipAModificar = GetById(chip.Id);
            chipAModificar = chip;
            return chip;
        }
    }
}
