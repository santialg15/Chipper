using ProyectoCompartido.Protocolo.FileTransfer;
using ProyectoCompartido.Protocolo.FileTransfer.FileHandler;
using ProyectoCompartido.Protocolo.FileTransfer.FileHandler.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace ProyectoCompartido.Protocolo
{
    public class NetworkDataHelper
    {
        private readonly NetworkStream networkStream;

        private readonly IFileStreamHandler _fileStreamHandler;
        private IFileHandler _fileHandler;

        public NetworkDataHelper(NetworkStream stream)
        {
            networkStream = stream;
            _fileStreamHandler = new FileStreamHandler();
        }

        public async Task SendMessage(string mensaje, int constant)
        {
            var header = new Header(HeaderConstants.Request, constant, mensaje.Length);
            var dataMessage = header.GetRequest();
            await networkStream.WriteAsync(dataMessage, 0, dataMessage.Length).ConfigureAwait(false);
            var bytesMessage = Encoding.UTF8.GetBytes(mensaje);
            await networkStream.WriteAsync(bytesMessage, 0, bytesMessage.Length).ConfigureAwait(false);
        }

        public async Task ReceiveData(byte[] buffer)
        {
            var Length = buffer.Length;
            var iRecv = 0;
            while (iRecv < Length)
            {
                var localRecv = await networkStream.ReadAsync(buffer, iRecv, Length - iRecv).ConfigureAwait(false);
                if (localRecv == 0) 
                {
                    throw new SocketException();
                }
                iRecv += localRecv;
                var dataLength = new byte[HeaderConstants.DataLength];
                var length = BitConverter.ToInt32(dataLength, 0);
                var data = new byte[length];
                var totalReceived = 0;
                while (totalReceived < length)
                {
                    var received = await networkStream.ReadAsync(data, totalReceived, length - totalReceived).ConfigureAwait(false);
                    if (received == 0)
                    {
                        throw new SocketException();
                    }
                    totalReceived += received;
                }
            }
        }

        public async Task SendFile(string path)
        {
            _fileHandler = new FileHandler();

            long fileSize = _fileHandler.GetFileSize(path);
            string fileName = _fileHandler.GetFileName(path);
            var header = new FileHeader().Create(fileName, fileSize);
            await Write(header); 

            await Write(Encoding.UTF8.GetBytes(fileName)); 

            long parts = SpecificationHelper.GetParts(fileSize);
            Console.WriteLine("Will Send {0} parts", parts);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = await _fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await _fileStreamHandler.Read(path, offset, Specification.MaxPacketSize);
                    offset += Specification.MaxPacketSize;
                }
                await Write(data); 
                currentPart++;
            }
        }

        public async Task<string> ReceiveFile()
        {
            var header = await Read(FileHeader.GetLength());
            var fileNameSize = BitConverter.ToInt32(header, 0);
            var fileSize = BitConverter.ToInt64(header, Specification.FixedFileNameLength);

            var fileName = Encoding.UTF8.GetString(Read(fileNameSize).Result);

            long parts = SpecificationHelper.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = await Read(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await Read(Specification.MaxPacketSize);
                    offset += Specification.MaxPacketSize;
                }
                await _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
            return fileName;
        }

        public async Task Write(byte[] data)
        {
           await networkStream.WriteAsync(data, 0, data.Length);
        }

        public async Task<byte[]> Read(int length)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = await networkStream.ReadAsync(data, dataReceived, length - dataReceived);
                if (received == 0)
                {
                    throw new SocketException();
                }
                dataReceived += received;
            }
            return data;
        }
    }
}
