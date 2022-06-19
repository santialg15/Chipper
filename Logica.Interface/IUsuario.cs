namespace Logica.Interface
{
    public interface IUsuario
    {
        List<Usuario> ObtenerTodos();
        Usuario Obtener(int idUsuario);
        Usuario Insertar(Usuario usuario);
        Usuario Modificar(Usuario usuario);
        void Eliminar(int idUsuario);
    }
}
