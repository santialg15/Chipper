using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAdminLogicInterface
{
    public interface ILogica<T,K>
    {
        IEnumerable<T> GetAll();
        T GetById(K id);
        T Insert(T entity);
        T Update(T entity);
        void Delete(K id);
    }

    public interface ILogicaUsuario : ILogica<Usuario,Guid> 
    {
        bool Exist(string nombre);
    }
    public interface ILogicaPublicaciones : ILogica<Publicacion, Guid> 
    {
        Respuesta CreateAnswer(Guid idPublicacion, Respuesta respuesta);
        void DeleteAnswer(Guid idPublicacion, Guid idRespuesta);

    }

}
