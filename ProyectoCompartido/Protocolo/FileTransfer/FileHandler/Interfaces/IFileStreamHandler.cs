namespace ProyectoCompartido.Protocolo.FileTransfer.FileHandler.Interfaces
{
    public interface IFileStreamHandler
    {
        Task<byte[]> Read(string path, long offset, int length);
        Task Write(string fileName, byte[] data);
    }
}