using Logica;

namespace ServerAdminGrpcInterface
{
    public interface IGrpcServerAdmin
    {
        void Bloquear(int idUsuario);
        void Desbloquear(int idUsuario);
        Usuario Insertar(Usuario usuario);
    }
}