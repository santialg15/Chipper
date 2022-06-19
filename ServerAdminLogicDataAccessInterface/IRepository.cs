using Logica;

namespace ServerAdminLogicDataAccessInterface
{
    public interface IRepository<T,K>
    {
        IEnumerable<T> GetAll();
        T GetById(K id);
        T Insert(T entity);
        T Update(T entity);
        void Delete(T entity);
    }

    public interface IUsersRepository : IRepository<Usuario,Guid> 
    {
        bool Exist(string id);
    }
    public interface IPublicacionesRepository : IRepository<Publicacion, Guid> 
    {
        void AddAnswer(Guid idPublicacion, Respuesta respuesta);
        void DeleteAnswer(Guid idPublicacion, Respuesta respuesta);
    }


}