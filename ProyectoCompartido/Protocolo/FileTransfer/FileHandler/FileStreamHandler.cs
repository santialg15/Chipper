using ProyectoCompartido.Protocolo.FileTransfer.FileHandler.Interfaces;

namespace ProyectoCompartido.Protocolo.FileTransfer.FileHandler
{
    public class FileStreamHandler : IFileStreamHandler
    {
        public async Task<byte[]> Read(string path, long offset, int length)
        {
            var data = new byte[length];

            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.Position = offset;
                var bytesRead = 0;
                while (bytesRead < length)
                {
                    var read = await fs.ReadAsync(data, bytesRead, length - bytesRead);
                    if (read == 0)
                    {
                        throw new Exception("Couldn't not read file");
                    }
                    bytesRead += read;
                }
            }

            return data;
        }

        public async Task Write(string fileName, byte[] data)
        {
            if (File.Exists(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Append))
                {
                    await fs.WriteAsync(data, 0, data.Length);
                }
            }
            else
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    await fs.WriteAsync(data, 0, data.Length);
                }
            }
        }
    }
}