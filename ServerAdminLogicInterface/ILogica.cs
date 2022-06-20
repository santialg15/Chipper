using Logica;

namespace ServerAdminLogicInterface
{
    public interface ILogica<T,K>
    {
        Task<List<T>> GetAll();
        Task<T> GetById(K name);
        Task<K> Insert(T entity);
        Task<K> Update(T entity);
        Task Delete(K name);
    }

    public interface ILogicaUsuario : ILogica<Usuario,string> 
    {
        Task ChangePermission(string name);
    }
    public interface ILogicaPublicaciones : ILogica<Publicacion, string> 
    {
        Task<Publicacion> GetById(Guid id);
        Task Delete(Guid idChip);
        Task<string> CreateAnswer(Guid idPublicacion, Respuesta respuesta);
        void DeleteAnswer(Guid idPublicacion, Guid idRespuesta);
    }

}
