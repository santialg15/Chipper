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

        //SendFile
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
                if (localRecv == 0) // Si recieve retorna 0 -> la conexion se cerro desde el endpoint remoto
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
        //////////////////////////////////////////////
        public async Task SendFile(string path)
        {
            _fileHandler = new FileHandler();

            // Obtiene el tamaño del archivo
            long fileSize = _fileHandler.GetFileSize(path);
            // Obtenemos el nombre del archivo
            string fileName = _fileHandler.GetFileName(path);
            var header = new FileHeader().Create(fileName, fileSize);
            await Write(header); // envia largo del nombre y tamaño del file

            await Write(Encoding.UTF8.GetBytes(fileName)); // Envia nombre del archivo

            long parts = SpecificationHelper.GetParts(fileSize);
            Console.WriteLine("Will Send {0} parts", parts);
            long offset = 0;
            long currentPart = 1;

            // Mientras tengo un segmento a enviar
            // 1 - Leo de disco el segmento
            // 2 - Guardo ese segmento en un buffer
            // 3 - Envio ese semgento a traves de la red
            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == parts)
                {
                    // leo el ultimo segmento
                    var lastPartSize = (int)(fileSize - offset);
                    data = _fileStreamHandler.Read(path, offset, lastPartSize); //Puntos 1 y 2 
                    offset += lastPartSize;
                }
                else
                {
                    // leo un segmento
                    data = _fileStreamHandler.Read(path, offset, Specification.MaxPacketSize); //Puntos 1 y 2
                    offset += Specification.MaxPacketSize;
                }
                await Write(data); // Punto 3
                currentPart++;
            }
        }

        public async Task<string> ReceiveFile()
        {
            //1 - Recibo el header
            var header = await Read(FileHeader.GetLength());
            // 2 - Me quedo con el largo del nombre del archivo
            var fileNameSize = BitConverter.ToInt32(header, 0);
            // 3 - Me quedo con el tamaño del file
            var fileSize = BitConverter.ToInt64(header, Specification.FixedFileNameLength);

            //4 - Recibo el nombre del archivo, usando el tamaño que recibi en el punto 2
            var fileName = Encoding.UTF8.GetString(Read(fileNameSize).Result);

            // 5 - Calculo cuantas partes voy a recibir
            long parts = SpecificationHelper.GetParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            // Mientras tengo partes para recibir
            // 1 - Me fijo si es la ultima parte
            // 1.1 - Si es, recibo la ultima parte
            // 2.2 - Si no, recibo una parte cualquiera
            // 3 - Escribo esa parte del archivo a disco
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
                _fileStreamHandler.Write(fileName, data);
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
        //////////////////////////////////////////////
    }
}
